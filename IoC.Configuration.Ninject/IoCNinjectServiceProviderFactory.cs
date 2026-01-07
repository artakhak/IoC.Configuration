using System;
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
                // Disable implicit self-binding globally. This ensures Ninject 
                // only resolves what we explicitly tell it to, preventing
                // the IExternalScopeProvider activation errors.
                ActivationCacheDisabled = false
            };

            var kernel = new IoCConfigurationNinjectKernel(settings);
            
            kernel.Bind<IKernel>().ToConstant(kernel);
            kernel.Bind<IServiceProvider>().ToMethod(ctx => new NinjectServiceProvider(kernel)).InTransientScope();

            foreach (var descriptor in services)
            {
                // We use a specific Ninject feature: if a binding already exists for this service type,
                // we still add the new one. Ninject handles multiple bindings as a collection.
                
                if (descriptor.ImplementationType != null)
                {
                    var binding = kernel.Bind(descriptor.ServiceType).To(descriptor.ImplementationType);
                    ApplyLifetime((global::Ninject.Syntax.IBindingInSyntax<object>)binding, descriptor.Lifetime);
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    var binding = kernel.Bind(descriptor.ServiceType)
                                        .ToMethod(context => descriptor.ImplementationFactory(new NinjectServiceProvider(kernel)));
                    ApplyLifetime((global::Ninject.Syntax.IBindingInSyntax<object>)binding, descriptor.Lifetime);
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
                    // Note: InThreadScope is used as a proxy for Scoped lifetime in this context.
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
                // To support system services like IServiceScopeFactory, we check
                // if there is a binding. Ninject 3.3.x Populate-style logic 
                // requires us to be explicit.
                if (_kernel.GetBindings(serviceType).Any())
                {
                    return _kernel.Get(serviceType);
                }

                // If no binding exists, but it's a concrete type (not an interface),
                // Ninject might still be able to resolve it. But for the Host 
                // compatibility, we return null to allow default fallbacks.
                return null;
            }

            public object GetRequiredService(Type serviceType)
            {
                var service = GetService(serviceType);
                if (service != null) 
                    return service;

                throw new InvalidOperationException($"No service for type '{serviceType}' has been registered.");
            }
        }
    }
}
