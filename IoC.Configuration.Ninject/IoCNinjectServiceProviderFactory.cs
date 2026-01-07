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
        [CanBeNull]
        public event EventHandler<IServiceProvider> OnServiceProviderCreated;

        [CanBeNull]
        public event EventHandler<IKernel> OnContainerBuilderCreated;

        public IKernel CreateBuilder(IServiceCollection services)
        {
            var kernel = new IoCConfigurationNinjectKernel();
            
            kernel.Bind<IKernel>().ToConstant(kernel);

            foreach (var descriptor in services)
            {
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
                else
                {
                    System.Console.WriteLine($@"Unhandled service [{descriptor.ServiceType.FullName ?? String.Empty}].");
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

        private class NinjectServiceProvider : IServiceProvider
        {
            private readonly IKernel _kernel;

            public NinjectServiceProvider(IKernel kernel)
            {
                _kernel = kernel;
            }

            public object GetService(Type serviceType)
            {
                // First, check if Ninject has an explicit binding.
                // Using GetBindings().Any() is safer than TryGet() for system types
                // that Ninject might try to "self-bind" incorrectly.
                if (_kernel.GetBindings(serviceType).Any())
                {
                    return _kernel.Get(serviceType);
                }

                // Fallback for system services that might not be explicitly bound in Ninject
                // but are available in the kernel's resolution root (like IServiceProvider itself)
                try
                {
                    return _kernel.TryGet(serviceType);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
