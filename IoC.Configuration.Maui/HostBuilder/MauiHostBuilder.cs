using IoC.Configuration.DiContainerBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Hosting;

namespace IoC.Configuration.Maui.HostBuilder;

/// <summary>
/// Maui implementation of <see cref="IApplicationHostBuilder{IHost}"/>
/// </summary>
public class MauiHostBuilder : IApplicationHostBuilder<MauiAppHost>
{
    private readonly MauiAppBuilder _mauiAppBuilder;
    private object? _factory;

    public MauiHostBuilder(MauiAppBuilder mauiAppBuilder)
    {
        _mauiAppBuilder = mauiAppBuilder;
    }

    public MauiAppHost Build()
    {
        var mauiAppHost = new MauiAppHost(_mauiAppBuilder.Build());
        return mauiAppHost;
    }

    /// <inheritdoc />
    public void UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : class
    {
        _mauiAppBuilder.ConfigureContainer(factory);
        _factory = factory;
    }

    /// <inheritdoc />
    public void ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate) where TContainerBuilder : notnull
    {
        if (_factory == null)
            throw new InvalidOperationException($"[{nameof(UseServiceProviderFactory)}] must be called before ConfigureContainer.");

        if (_factory is not IServiceProviderFactory<TContainerBuilder> genericFactory)
            throw new InvalidOperationException($"The factory is expected to be of type [{typeof(IServiceProviderFactory<TContainerBuilder>)}]. Actual type is [{_factory.GetType()}].");

        _mauiAppBuilder.ConfigureContainer(genericFactory, (builder) =>
            configureDelegate(null!, builder));
    }
}
