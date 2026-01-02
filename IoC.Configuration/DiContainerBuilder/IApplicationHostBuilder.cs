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
        IApplicationHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : class;

        /// <summary>
        /// Enables configuring the instantiated dependency container. This can be called multiple times and
        /// the results will be additive.
        /// </summary>
        /// <typeparam name="TContainerBuilder">The type of builder.</typeparam>
        /// <param name="configureDelegate">The delegate which configures the builder.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        IApplicationHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate);


        //void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory,
        //    Action<Microsoft.Extensions.Hosting.HostBuilderContext, TContainerBuilder> configureDelegate) where TContainerBuilder : class;

        /*var serviceProviderFactory = new OROptimizerAutofacServiceProviderFactory(new AutofacServiceProviderFactory());
           serviceProviderFactory.OnServiceProviderCreated += (sender, e) =>
           {
               ServiceProviderAmbientIoCContext.InitializeContext(new MicrosoftServiceProvider(e));
           };

           //serviceProviderFactory.CreateBuilder()
           //serviceProviderFactory.CreateServiceProvider()


           State.WebApplicationBuilder.Host.UseServiceProviderFactory(serviceProviderFactory)
               .ConfigureContainer<ContainerBuilder>((context, containerBuilder) =>
               {
                   foreach (var module in getModules(this.State))
                       containerBuilder.RegisterModule(module);
               });*/


    }
}