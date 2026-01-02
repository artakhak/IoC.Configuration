using IoC.Configuration.DiContainerBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.AspNetConfigureHostBuilder;

/// <inheritdoc />
public class AspNetApplicationHostBuilder: IApplicationHostBuilder
{
    private readonly IHostBuilder _hostBuilder;

    public AspNetApplicationHostBuilder(IHostBuilder hostBuilder)
    {
        _hostBuilder = hostBuilder;
    }

    public IApplicationHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : class
    {
        _hostBuilder.UseServiceProviderFactory(factory);
        return this;
    }

    public IApplicationHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _hostBuilder.ConfigureContainer(configureDelegate);
        return this;
    }
}