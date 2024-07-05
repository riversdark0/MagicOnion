using System.Reflection;
using Cysharp.Runtime.Multicast;
using Cysharp.Runtime.Multicast.InMemory;
using Cysharp.Runtime.Multicast.Remoting;
using Grpc.AspNetCore.Server.Model;
using MagicOnion.Server;
using MagicOnion.Server.Diagnostics;
using MagicOnion.Server.Glue;
using MagicOnion.Server.Hubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MagicOnionServicesExtensions
{
    public static IMagicOnionServerBuilder AddMagicOnion(this IServiceCollection services, Action<MagicOnionOptions>? configureOptions = null)
    {
        var configName = Options.Options.DefaultName;
        services.TryAddSingleton<MagicOnionServiceDefinition>(sp => MagicOnionEngine.BuildServerServiceDefinition(sp, sp.GetRequiredService<IOptionsMonitor<MagicOnionOptions>>().Get(configName)));
        return services.AddMagicOnionCore(configureOptions);
    }

    public static IMagicOnionServerBuilder AddMagicOnion(this IServiceCollection services, Assembly[] searchAssemblies, Action<MagicOnionOptions>? configureOptions = null)
    {
        var configName = Options.Options.DefaultName;
        services.TryAddSingleton<MagicOnionServiceDefinition>(sp => MagicOnionEngine.BuildServerServiceDefinition(sp, searchAssemblies, sp.GetRequiredService<IOptionsMonitor<MagicOnionOptions>>().Get(configName)));
        return services.AddMagicOnionCore(configureOptions);
    }

    public static IMagicOnionServerBuilder AddMagicOnion(this IServiceCollection services, IEnumerable<Type> searchTypes, Action<MagicOnionOptions>? configureOptions = null)
    {
        var configName = Options.Options.DefaultName;
        services.TryAddSingleton<MagicOnionServiceDefinition>(sp => MagicOnionEngine.BuildServerServiceDefinition(sp, searchTypes, sp.GetRequiredService<IOptionsMonitor<MagicOnionOptions>>().Get(configName)));
        return services.AddMagicOnionCore(configureOptions);
    }

    // NOTE: `internal` is required for unit tests.
    internal static IMagicOnionServerBuilder AddMagicOnionCore(this IServiceCollection services, Action<MagicOnionOptions>? configureOptions = null)
    {
        var configName = Options.Options.DefaultName;

        // Required services (ASP.NET Core, gRPC)
        services.AddLogging();
        services.AddGrpc();
        services.AddMetrics();

        // MagicOnion: Core services
        var glueServiceType = MagicOnionGlueService.CreateType();

        services.TryAddSingleton<StreamingHubHandlerRepository>();
        services.TryAddSingleton<MagicOnionServiceDefinitionGlueDescriptor>(sp => new MagicOnionServiceDefinitionGlueDescriptor(glueServiceType, sp.GetRequiredService<MagicOnionServiceDefinition>()));
        services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IServiceMethodProvider<>).MakeGenericType(glueServiceType), typeof(MagicOnionGlueServiceMethodProvider<>).MakeGenericType(glueServiceType)));

        // MagicOnion: Metrics
        services.TryAddSingleton<MagicOnionMetrics>();

        // MagicOnion: Options
        services.AddOptions<MagicOnionOptions>(configName)
            .Configure<IConfiguration>((o, configuration) =>
            {
                configuration.GetSection(string.IsNullOrWhiteSpace(configName) ? "MagicOnion" : configName).Bind(o);
                configureOptions?.Invoke(o);
            });

        // Add: Multicaster
        services.TryAddSingleton<IInMemoryProxyFactory>(DynamicInMemoryProxyFactory.Instance);
        services.TryAddSingleton<IRemoteProxyFactory>(DynamicRemoteProxyFactory.Instance);
        services.TryAddSingleton<IRemoteSerializer, MagicOnionRemoteSerializer>();
        services.TryAddSingleton<IRemoteClientResultPendingTaskRegistry, RemoteClientResultPendingTaskRegistry>();
        services.TryAddSingleton<IMulticastGroupProvider, RemoteGroupProvider>();
        services.TryAddSingleton<MagicOnionManagedGroupProvider>();

        return new MagicOnionServerBuilder(services);
    }
}
