using IoC.Configuration.ConfigurationFile;
using Microsoft.Extensions.DependencyInjection;

namespace IoC.Configuration.AspNet;

/// <summary>
/// Helpers for controller registrations.
/// </summary>
public static class ControllerRegistrationHelpers
{
    /// <summary>
    /// Registers controllers specified in <see cref="IConfiguration.WebApi"/> and <see cref="IConfiguration.PluginsSetup"/>.<see cref="IPluginSetup.WebApi"/>
    /// in IoC.Configuration file. 
    /// </summary>
    /// <param name="iocConfiguration">A structure generated from the data in IoC.Configuration file.</param>
    /// <param name="controllerBuilder">
    /// Controller builder. Controllers in <paramref name="iocConfiguration"/> will be added to <paramref name="controllerBuilder"/>.
    /// </param>
    public static void RegisterControllers(IConfiguration iocConfiguration, IMvcBuilder controllerBuilder)
    {
        void AddControllersFromWebApi(IWebApi? webApi)
        {
            if (webApi?.ControllerAssemblies?.Assemblies == null)
                return;

            foreach (var webApiControllerAssembly in webApi.ControllerAssemblies.Assemblies)
            {
                if (webApiControllerAssembly.LoadedAssembly != null)
                    controllerBuilder.AddApplicationPart(webApiControllerAssembly.LoadedAssembly);
            }
        }

        AddControllersFromWebApi(iocConfiguration.WebApi);

        if (iocConfiguration.PluginsSetup != null)
        {
            foreach (var pluginSetup in iocConfiguration.PluginsSetup.AllPluginSetups.Where(x => x.Enabled))
                AddControllersFromWebApi(pluginSetup.WebApi);
        }
    }
}
