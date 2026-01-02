using IoC.Configuration.DiContainerBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.AspNetConfigureHostBuilder;

/// <inheritdoc />
public class AspNetApplicationHostBuilder: IApplicationHostBuilder
{
    private readonly ConfigureHostBuilder _configureHostBuilder;

    public AspNetApplicationHostBuilder(ConfigureHostBuilder configureHostBuilder)
    {
        _configureHostBuilder = configureHostBuilder;
    }

    public IApplicationHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : class
    {
        _configureHostBuilder.UseServiceProviderFactory(factory);
        return this;
    }

    public IApplicationHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _configureHostBuilder.ConfigureContainer(configureDelegate);
        return this;
    }
}