using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Ninject;

namespace IoC.Configuration.Ninject
{
    /// <inheritdoc />
    // ReSharper disable once InconsistentNaming
    public class IoCNinjectServiceProviderFactory : IServiceProviderFactory<IKernel>
    {
        [CanBeNull] public event EventHandler<IServiceProvider> OnServiceProviderCreated;

        [CanBeNull] public event EventHandler<IKernel> OnContainerBuilderCreated;

        public IKernel CreateBuilder(IServiceCollection services)
        {
            var settings = new NinjectSettings
            {
                AllowNullInjection = true,
                ActivationCacheDisabled = false
            };

            var kernel = new IoCConfigurationNinjectKernel(settings);

            kernel.Bind<IKernel>().ToConstant(kernel);
            kernel.Bind<IServiceProvider>().ToMethod(ctx => new NinjectServiceProvider(kernel)).InTransientScope();
            
            // Explicitly register IServiceScopeFactory. Ninject needs this to support 
            // the .NET scope infrastructure used by Swagger and Web API.
            kernel.Bind<IServiceScopeFactory>().ToMethod(ctx => new NinjectServiceScopeFactory(kernel)).InSingletonScope();

            foreach (var descriptor in services)
            {
                if (descriptor.ImplementationType != null)
                {
                    var binding = kernel.Bind(descriptor.ServiceType).To(descriptor.ImplementationType);
                    ApplyLifetime((global::Ninject.Syntax.IBindingInSyntax<object>) binding, descriptor.Lifetime);
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    var binding = kernel.Bind(descriptor.ServiceType)
                        .ToMethod(context => descriptor.ImplementationFactory(new NinjectServiceProvider(kernel)));
                    ApplyLifetime((global::Ninject.Syntax.IBindingInSyntax<object>) binding, descriptor.Lifetime);
                }
                else if (descriptor.ImplementationInstance != null)
                {
                    kernel.Bind(descriptor.ServiceType).ToConstant(descriptor.ImplementationInstance);
                }
            }

            OnContainerBuilderCreated?.Invoke(this, kernel);
            return kernel;
        }

        private void ApplyLifetime(global::Ninject.Syntax.IBindingInSyntax<object> binding, ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    binding.InSingletonScope();
                    break;
                case ServiceLifetime.Transient:
                    binding.InTransientScope();
                    break;
                case ServiceLifetime.Scoped:
                    binding.InThreadScope();
                    break;
            }
        }

        public IServiceProvider CreateServiceProvider(IKernel containerBuilder)
        {
            IServiceProvider serviceProvider = new NinjectServiceProvider(containerBuilder);
            OnServiceProviderCreated?.Invoke(this, serviceProvider);
            return serviceProvider;
        }

        private class NinjectServiceProvider : IServiceProvider, ISupportRequiredService
        {
            private readonly IKernel _kernel;

            public NinjectServiceProvider(IKernel kernel)
            {
                _kernel = kernel;
            }

            public object GetService(Type serviceType)
            {
                // .NET expectation: IEnumerable<T> should always return an instance (even if empty),
                // while single services should return null if not registered.
                bool isCollection = serviceType.IsGenericType && 
                                    (serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                                     serviceType.GetGenericTypeDefinition() == typeof(IList<>) ||
                                     serviceType.GetGenericTypeDefinition() == typeof(ICollection<>));

                if (isCollection || serviceType.IsArray || _kernel.GetBindings(serviceType).Any())
                {
                    // Ninject's Get() will return the instance for explicit bindings,
                    // or an empty collection for the collection types above.
                    return _kernel.Get(serviceType);
                }

                // Return null for unregistered single types to allow Host fallbacks
                // and prevent the IExternalScopeProvider activation crash.
                return null;
            }

            public object GetRequiredService(Type serviceType)
            {
                var service = GetService(serviceType);
                if (service != null) return service;

                throw new InvalidOperationException($"No service for type '{serviceType}' has been registered.");
            }
        }

        private class NinjectServiceScopeFactory : IServiceScopeFactory
        {
            private readonly IKernel _kernel;
            public NinjectServiceScopeFactory(IKernel kernel) => _kernel = kernel;
            public IServiceScope CreateScope() => new NinjectServiceScope(_kernel);
        }

        private class NinjectServiceScope : IServiceScope
        {
            public IServiceProvider ServiceProvider { get; }
            public NinjectServiceScope(IKernel kernel) => ServiceProvider = new NinjectServiceProvider(kernel);
            public void Dispose() { /* Scope cleanup if necessary */ }
        }
    }
}
