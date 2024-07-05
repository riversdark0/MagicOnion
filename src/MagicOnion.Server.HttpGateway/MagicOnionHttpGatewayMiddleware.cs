using System.Buffers;
using Grpc.Core;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Text;
using Grpc.Net.Client;
using MagicOnion.Internal;
using Newtonsoft.Json;
using MessagePack;

namespace MagicOnion.Server.HttpGateway;

public class MagicOnionHttpGatewayMiddleware
{
    readonly RequestDelegate next;
    readonly IDictionary<string, MethodHandler> handlers;
    readonly GrpcChannel channel;

    public MagicOnionHttpGatewayMiddleware(RequestDelegate next, IReadOnlyList<MethodHandler> handlers, GrpcChannel channel)
    {
        this.next = next;
        this.handlers = handlers.ToDictionary(x => x.ToString());
        this.channel = channel;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            var path = $"{httpContext.Request.RouteValues["service"]}/{httpContext.Request.RouteValues["method"]}";

            MethodHandler handler;
            if (!handlers.TryGetValue(path, out handler))
            {
                httpContext.Response.StatusCode = 404;
                httpContext.Response.ContentType = "text/plain";
                await httpContext.Response.WriteAsync("Not Found");
                return;
            }

            // from form...
            object deserializedObject;
            if (httpContext.Request.ContentType == "application/x-www-form-urlencoded")
            {
                //object parameters
                var args = new List<object>();
                var typeArgs = new List<Type>();

                foreach (var p in handler.MethodInfo.GetParameters())
                {
                    typeArgs.Add(p.ParameterType);

                    StringValues stringValues;
                    if (httpContext.Request.Form.TryGetValue(p.Name, out stringValues))
                    {
                        if (p.ParameterType == typeof(string))
                        {
                            args.Add((string)stringValues);
                        }
                        else if (p.ParameterType.GetTypeInfo().IsEnum)
                        {
                            args.Add(Enum.Parse(p.ParameterType, (string)stringValues));
                        }
                        else
                        {
                            var collectionType = GetCollectionType(p.ParameterType);
                            if (stringValues.Count == 1 || collectionType == null)
                            {
                                var values = (string)stringValues;
                                if (p.ParameterType == typeof(DateTime) || p.ParameterType == typeof(DateTimeOffset) || p.ParameterType == typeof(DateTime?) || p.ParameterType == typeof(DateTimeOffset?))
                                {
                                    values = "\"" + values + "\"";
                                }

                                args.Add(JsonConvert.DeserializeObject(values, p.ParameterType));
                            }
                            else
                            {
                                string serializeTarget;
                                if (collectionType == typeof(string))
                                {
                                    serializeTarget = "[" + string.Join(", ", stringValues.Select(x => JsonConvert.SerializeObject(x))) + "]"; // escape serialzie
                                }
                                else if (collectionType.GetTypeInfo().IsEnum || collectionType == typeof(DateTime) || collectionType == typeof(DateTimeOffset) || collectionType == typeof(DateTime?) || collectionType == typeof(DateTimeOffset?))
                                {
                                    serializeTarget = "[" + string.Join(", ", stringValues.Select(x => "\"" + x + "\"")) + "]";
                                }
                                else
                                {
                                    serializeTarget = "[" + (string)stringValues + "]";
                                }

                                args.Add(JsonConvert.DeserializeObject(serializeTarget, p.ParameterType));
                            }
                        }
                    }
                    else
                    {
                        if (p.HasDefaultValue)
                        {
                            args.Add(p.DefaultValue);
                        }
                        else
                        {
                            args.Add(null);
                        }
                    }
                }

                deserializedObject = typeArgs.Count == 1 ?
                    args[0] : MagicOnionMarshallers.InstantiateDynamicArgumentTuple(typeArgs.ToArray(), args.ToArray());
            }
            else
            {
                string body;
                using (var sr = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
                {
                    body = await sr.ReadToEndAsync();
                }

                if (handler.RequestType == typeof(byte[]) && string.IsNullOrWhiteSpace(body))
                {
                    body = "[]";
                }

                if (handler.RequestType == typeof(Nil) && string.IsNullOrWhiteSpace(body))
                {
                    deserializedObject = Nil.Default;
                }
                else
                {
                    deserializedObject = Newtonsoft.Json.JsonConvert.DeserializeObject(body, handler.RequestType);
                }
            }

            // JSON to C# Object to MessagePack
            var bufferWriter = new ArrayBufferWriter<byte>();
            handler.MessageSerializer.Serialize(bufferWriter, deserializedObject);
            var requestObject = bufferWriter.WrittenSpan.ToArray();

            var throughMarshaller = new Marshaller<byte[]>(x => x, x => x);
            var method = new Method<byte[], byte[]>(MethodType.Unary, handler.ServiceName, handler.MethodName, throughMarshaller, throughMarshaller);

            // create header
            var metadata = new Metadata();
            foreach (var header in httpContext.Request.Headers.Where(x => !x.Key.StartsWith(":")))
            {
                metadata.Add(header.Key, header.Value);
            }

            var invoker = channel.CreateCallInvoker();
            var rawResponse = await invoker.AsyncUnaryCall(method, null, default(CallOptions).WithHeaders(metadata), requestObject);

            // MessagePack -> Object -> Json
            var obj = Deserialize(handler, handler.UnwrappedResponseType, rawResponse);
            var v = JsonConvert.SerializeObject(obj, new[] { new Newtonsoft.Json.Converters.StringEnumConverter() });
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(v);
        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = 500;
            httpContext.Response.ContentType = "text/plain";
            await httpContext.Response.WriteAsync(ex.ToString());
        }
    }

    static object Deserialize(MethodHandler handler, Type t, byte[] serializedData)
    {
        var method = typeof(MagicOnionHttpGatewayMiddleware).GetMethod(nameof(DeserializeCore), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(t);
        return method.Invoke(null, new object[] { handler, serializedData });
    }
    static T DeserializeCore<T>(MethodHandler handler, byte[] serializedData)
    {
        return handler.MessageSerializer.Deserialize<T>(new ReadOnlySequence<byte>(serializedData));
    }

    Type GetCollectionType(Type type)
    {
        if (type.IsArray) return type.GetElementType();

        if (type.GetTypeInfo().IsGenericType)
        {
            var genTypeDef = type.GetGenericTypeDefinition();
            if (genTypeDef == typeof(IEnumerable<>)
                || genTypeDef == typeof(ICollection<>)
                || genTypeDef == typeof(IList<>)
                || genTypeDef == typeof(List<>)
                || genTypeDef == typeof(IReadOnlyCollection<>)
                || genTypeDef == typeof(IReadOnlyList<>))
            {
                return genTypeDef.GetGenericArguments()[0];
            }
        }

        return null; // not collection
    }
}
