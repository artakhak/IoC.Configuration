using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace IoC.Configuration.Autofac
{
    /// <inheritdoc />
    // ReSharper disable once InconsistentNaming
    internal class IoCAutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly AutofacServiceProviderFactory _autofacServiceProviderFactory;

        [CanBeNull]
        public event EventHandler<IServiceProvider> OnServiceProviderCreated;

        [CanBeNull]
        public event EventHandler<ContainerBuilder> OnContainerBuilderCreated;

        // ReSharper disable once ConvertToPrimaryConstructor
        public IoCAutofacServiceProviderFactory(AutofacServiceProviderFactory autofacServiceProviderFactory)
        {
            _autofacServiceProviderFactory = autofacServiceProviderFactory;
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var containerBuilder = _autofacServiceProviderFactory.CreateBuilder(services);
            OnContainerBuilderCreated?.Invoke(this, containerBuilder);
            return containerBuilder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var serviceProvider = _autofacServiceProviderFactory.CreateServiceProvider(containerBuilder);

            OnServiceProviderCreated?.Invoke(this, serviceProvider);
            return serviceProvider;
        }
    }
}