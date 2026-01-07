using IoC.Configuration.DiContainerBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.AspNet.HostBuilder;

public class WebApplicationHostBuilder : IApplicationHostBuilder<WebApplication>
{
    private readonly WebApplicationBuilder _webApplicationBuilder;

    public WebApplicationHostBuilder(WebApplicationBuilder webApplicationBuilder)
    {
        _webApplicationBuilder = webApplicationBuilder;
    }

    /// <inheritdoc />
    public void UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : class
    {
        _webApplicationBuilder.Host.UseServiceProviderFactory(factory);
    }

    /// <inheritdoc />
    public void ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _webApplicationBuilder.Host.ConfigureContainer(configureDelegate);
    }

    /// <inheritdoc />
    public WebApplication Build()
    {
        return _webApplicationBuilder.Build();
    }
}