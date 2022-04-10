// // Used to edit the demo code in 'docs/application-integration/asp-net-core.generated.rst'
// // Uncomment the code make changes, and copy the new code to docs/application-integration\asp-net-core.rst.template
// using System;
// using System.IO;
// using IoC.Configuration.DiContainerBuilder;
// using IoC.Configuration.DiContainerBuilder.FileBased;
// using OROptimizer;
//
// namespace IoC.Configuration.Tests.DocumentationTests;
//
// public class SetupForAspNetCoreDemo
// {
//     private static IContainerInfo _containerInfo;
//
//     public IServiceProvider ConfigureServices(IServiceCollection services)
//     {
//         // Do some ASP.NET Core configuration
//         var mvcBuilder = services.AddMvc()
//             .AddMvcOptions(options =>
//             {
//                 // ...
//             })
//             .AddJsonOptions(options =>
//             {
//                 // ...
//             })
//             .AddControllersAsServices();
//
//         var rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
//
//         // Load IoC.Configuration into iocContainerBuilder
//         var iocContainerBuilder =
//             new DiContainerBuilder()
//                 .StartFileBasedDi(new FileBasedConfigurationParameters(new FileBasedConfigurationFileContentsProvider(
//                         Path.Combine(rootDirectory, "WebFileSystem.IoC.Configuration.xml")),
//                     rootDirectory, new AllLoadedAssemblies())
//                 {
//                     ConfigurationFileXmlDocumentLoaded = (sender, e) =>
//                     {
//                         // Do XML file transformations here
//                     }
//                 }, out var loadedConfiguration);
//
//         // Register controller assemblies in webApi elements in IoC.Configuration file
//         // with ASP.NET Core.
//         Action<IoC.Configuration.ConfigurationFile.IWebApi> addControllersFromConfiguration =
//             (webApi) =>
//             {
//                 if (webApi == null || webApi.ControllerAssemblies == null)
//                     return;
//
//                 foreach (var controllerAssembly in webApi.ControllerAssemblies.Assemblies)
//                 {
//                     if (controllerAssembly.LoadedAssembly != null)
//                         mvcBuilder.AddApplicationPart(controllerAssembly.LoadedAssembly);
//                 }
//             };
//
//         // Register controller assemblies in iocConfiguration/webApi element.
//         addControllersFromConfiguration(loadedConfiguration.WebApi);
//
//         // Now register controller assemblies in webApi elements under
//         // iocConfiguration/pluginsSetup/pluginSetup elements.
//         foreach (var pluginSetup in loadedConfiguration.PluginsSetup.AllPluginSetups)
//         {
//             if (pluginSetup.Enabled)
//                 addControllersFromConfiguration(pluginSetup.WebApi);
//         }
//
//         // Build the Autofac container builder and start the IoC.Configuration.
//         var autofacContainerBuilder = new ContainerBuilder();
//
//         // Register ASP.NET Core services with Autofac, however skip
//         // the services, the full name of which starts with "WebFileSystemApi".
//         // Registering bindings of non-Microsoft services will be done in
//         // IoC.Configuration file.
//         autofacContainerBuilder.Populate(
//             services.Where(x =>
//                 !x.ServiceType.FullName.StartsWith("WebFileSystemApi", StringComparison.Ordinal)));
//
//         // Since we provide an instance of
//         // IoC.Configuration.Autofac.AutofacDiContainer,
//         // IoC.Configuration.Autofac will not create and build instance of
//         // Autofac.ContainerBuilder.
//         // In this case, we need to call iocContainerStarter.Start() only after
//         // we call autofacContainerBuilder.Build() below.
//         var iocContainerStarter = iocContainerBuilder
//             .WithDiContainer(new AutofacDiContainer(autofacContainerBuilder))
//             .RegisterModules();
//
//         var container = autofacContainerBuilder.Build();
//         _containerInfo = iocContainerStarter.Start();
//
//         return new AutofacServiceProvider(container);
//     }
// }