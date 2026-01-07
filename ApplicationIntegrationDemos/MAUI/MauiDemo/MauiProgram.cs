using IoC.Configuration;
using IoC.Configuration.DiContainerBuilder;
using IoC.Configuration.DiContainerBuilder.FileBased;
using IoC.Configuration.Maui.HostBuilder;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using Settings = MauiDemo.Properties.Settings;

namespace MauiDemo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        LogHelper.RegisterContext(new LogHelperContextLogToConsole());

        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        var (mauiApp, containerInfo) = ConfigureDiAndBuildMauiApp(builder);
        return mauiApp;
    }

    private static (MauiApp mauiApp, IContainerInfo) ConfigureDiAndBuildMauiApp(MauiAppBuilder mauiAppBuilder)
    {
        var diContainerBuilder = new DiContainerBuilder();

        var fileBasedConfigurationParameters = new FileBasedConfigurationParameters(
            new FileBasedConfigurationFileContentsProvider(Settings.Default.IoCConfigurationFilePath),
            AppContext.BaseDirectory, new AllLoadedAssemblies())
        {
            AttributeValueTransformers = [new FileFolderPathAttributeValueTransformer()]
        };

        var hostIntegratedContainerInfo = diContainerBuilder.StartFileBasedDi(fileBasedConfigurationParameters)
            .WithoutPresetDiContainer()
            // Add additional modules using AddAdditionalDiModules() one or multiple times as necessary
            // to register modules in addition to DI specified in "IoCConfiguration.xml"
            // If the method is not called, only the 
            //.AddAdditionalDiModules(new MyModule())

            // Use WithHostBuilder(hostBuilder) to make sure IoC.Configuration will register DI with the host builder
            // Do not call hostBuilder.Build() since this will be done by IoC.Configuration.
            .WithHostBuilder(new MauiHostBuilder(mauiAppBuilder))
            .RegisterServiceProviderAndBuildApp();

        // var diContainer = hostIntegratedContainerInfo.ContainerInfo.DiContainer;
        // From this point on either AppHost.Services or hostIntegratedContainerInfo.ContainerInfo.DiContainer
        // can be used to resolve services. Both will use the same DI container.

        return (hostIntegratedContainerInfo.Host.MauiApp, hostIntegratedContainerInfo.ContainerInfo);
    }
}

