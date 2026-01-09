//using Microsoft.Extensions.DependencyInjection;
using IoC.Configuration;
using IoC.Configuration.DiContainerBuilder;
using IoC.Configuration.DiContainerBuilder.FileBased;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using OROptimizer;

using System;
using OROptimizer.Diagnostics.Log;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3Demo;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    /// <summary>
    /// Initializes the singleton application object.
    /// </summary>
    public App()
    {
        //Services = ConfigureServices();
        this.InitializeComponent();

        LogHelper.RegisterContext(new LogHelperContextLogToConsole());
        ConfigureDi();
    }

    private static void ConfigureDi()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        var diContainerBuilder = new IoC.Configuration.DiContainerBuilder.DiContainerBuilder();

        var fileBasedConfigurationParameters = new FileBasedConfigurationParameters(
            new FileBasedConfigurationFileContentsProvider(WinUI3Demo.Properties.Settings.Default.IoCConfigurationFilePath),
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
            .WithHostBuilder(new ApplicationHostBuilder(hostBuilder))
            .RegisterServiceProviderAndBuildApp();

        AppHost = hostIntegratedContainerInfo.Host;

        var diContainer = hostIntegratedContainerInfo.ContainerInfo.DiContainer;
        // From this point on either AppHost.Services or hostIntegratedContainerInfo.ContainerInfo.DiContainer
        // can be used to resolve services. Both will use the same DI container.
    }
    
    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        // Resolve MainWindow from the DI container
        await (AppHost ?? throw new InvalidOperationException("Hos not set")).StartAsync();

        var window = AppHost.Services.GetRequiredService<MainWindow>();
        window.Activate();
    }
}
