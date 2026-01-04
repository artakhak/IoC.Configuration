using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.DiContainerBuilder
{
    public interface IApplicationHostBuilder
    {
        /// <summary>
        /// Overrides the factory used to create the service provider.
        /// </summary>
        /// <typeparam name="TContainerBuilder">The type of builder.</typeparam>
        /// <param name="factory">The factory to register.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        void UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : class;

        /// <summary>
        /// Enables configuring the instantiated dependency container. This can be called multiple times and
        /// the results will be additive.
        /// </summary>
        /// <typeparam name="TContainerBuilder">The type of builder.</typeparam>
        /// <param name="configureDelegate">The delegate which configures the builder.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        void ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate);
    }

    public interface IApplicationHostBuilder<out THost>: IApplicationHostBuilder where THost: class, IHost
    {
        /// <summary>
        /// Builds host of type <typeparamref name="THost"/>.
        /// </summary>
        /// <returns></returns>
        THost Build();
    }
}