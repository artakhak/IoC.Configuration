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
                // .NET expectation: IEnumerable<T> should always return an instance (even if empty).
                bool isCollection = serviceType.IsGenericType && 
                                    (serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                                     serviceType.GetGenericTypeDefinition() == typeof(IList<>) ||
                                     serviceType.GetGenericTypeDefinition() == typeof(ICollection<>));

                if (isCollection || serviceType.IsArray)
                {
                    // Ninject handles collection types by returning an empty list if no bindings exist.
                    return _kernel.Get(serviceType);
                }

                // IMPORTANT: For interfaces, we MUST check if a binding exists.
                // If no binding exists for an interface, returning null allows Microsoft's 
                // internal factories (like LoggerFactory) to use their own fallbacks.
                // This prevents the 'IServiceProviderIsService' and 'IExternalScopeProvider' activation crashes.
                var bindings = _kernel.GetBindings(serviceType).ToList();
                
                if (bindings.Any())
                {
                    // If multiple bindings exist, .NET expects the LAST one.
                    if (bindings.Count > 1)
                    {
                        return _kernel.GetAll(serviceType).LastOrDefault();
                    }

                    return _kernel.Get(serviceType);
                }

                // If it's a concrete class and not an interface, Ninject might be able 
                // to self-bind it. But for safety with Hosting, we return null for interfaces.
                return null;
            }

            public object GetRequiredService(Type serviceType)
            {
                var service = GetService(serviceType);
                if (service != null) return service;

                // If GetService returned null for a collection, it's a bug in the logic above.
                // If it returned null for a single type, it's truly missing.
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
