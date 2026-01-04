using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.DiContainerBuilder
{
    public class ApplicationHostBuilder : IApplicationHostBuilder<IHost>
    {
        private readonly IHostBuilder _hostBuilder;

        public ApplicationHostBuilder(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
        }

        /// <inheritdoc />
        public void UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : class
        {
            _hostBuilder.UseServiceProviderFactory(factory);
        }

        /// <inheritdoc />
        public void ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            _hostBuilder.ConfigureContainer(configureDelegate);
        }

        /// <inheritdoc />
        public IHost Build()
        {
            return _hostBuilder.Build();
        }
    }
}