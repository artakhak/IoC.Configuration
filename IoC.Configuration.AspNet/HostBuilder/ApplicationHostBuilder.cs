using IoC.Configuration.DiContainerBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.AspNet.HostBuilder;

/// <summary>
/// A builder for configuring and constructing a <see cref="WebApplication"/> instance.
/// Provides methods to configure the underlying dependency injection container
/// and build the application for hosting.
/// </summary>
public class WebApplicationHostBuilder : IApplicationHostBuilder<WebApplication>
{
    private readonly WebApplicationBuilder _webApplicationBuilder;

    /// <summary>
    /// Represents a builder for configuring and creating an instance of <see cref="WebApplication"/>.
    /// Provides functionality for integrating a custom dependency injection container
    /// and configuring application services.
    /// </summary>
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
