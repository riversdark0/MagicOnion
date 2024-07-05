using System;
using System.Threading.Tasks;

namespace MagicOnion.Client
{
    /// <summary>
    /// [Preview] The interface of the handler for StreamingHub diagnostics. This API may change in the future.
    /// </summary>
    public interface IStreamingHubDiagnosticHandler
    {
        public delegate ValueTask<TResponse> InvokeMethodDelegate<TRequest, TResponse>(int methodId, TRequest value);

        /// <summary>
        /// The callback method at the beginning of a Hub method request. This API may change in the future.
        /// </summary>
        /// <typeparam name="THub"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="hubInstance"></param>
        /// <param name="methodId"></param>
        /// <param name="methodName"></param>
        /// <param name="request"></param>
        /// <param name="isFireAndForget"></param>
        /// <param name="invokeMethod"></param>
        ValueTask<TResponse> OnMethodInvoke<THub, TRequest, TResponse>(THub hubInstance, int methodId, string methodName, TRequest request, bool isFireAndForget, InvokeMethodDelegate<TRequest, TResponse> invokeMethod);

        /// <summary>
        /// [Preview] The callback method when a method of HubReceiver is invoked. This API may change in the future.
        /// </summary>
        /// <typeparam name="THub"></typeparam>
        /// <param name="hubInstance"></param>
        /// <param name="methodId"></param>
        /// <param name="data"></param>
        void OnBroadcastEventRaw<THub>(THub hubInstance, int methodId, ReadOnlyMemory<byte> data);

        /// <summary>
        /// [Preview] The callback method when a method of HubReceiver is invoked. This API may change in the future.
        /// </summary>
        /// <typeparam name="THub"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="hubInstance"></param>
        /// <param name="methodName"></param>
        /// <param name="value"></param>
        void OnBroadcastEvent<THub, T>(THub hubInstance, string methodName, T value);

        /// <summary>
        /// [Preview] The callback method when the return value of a hub method call is received. This API may change in the future.
        /// </summary>
        /// <typeparam name="THub"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="hubInstance"></param>
        /// <param name="methodName"></param>
        /// <param name="data"></param>
        void OnResponseEvent<THub, T>(THub hubInstance, string methodName, ReadOnlyMemory<byte> data);
    }
}
