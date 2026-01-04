using Microsoft.UI.Xaml;
// using Microsoft.UI.Xaml.Controls;
// using Microsoft.UI.Xaml.Controls.Primitives;
// using Microsoft.UI.Xaml.Data;
// using Microsoft.UI.Xaml.Input;
// using Microsoft.UI.Xaml.Media;
// using Microsoft.UI.Xaml.Navigation;
// using Microsoft.UI.Xaml.Shapes;
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Runtime.InteropServices.WindowsRuntime;
// using Windows.ApplicationModel;
// using Windows.ApplicationModel.Activation;
// using Windows.Foundation;
// using Windows.Foundation.Collections;

using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Shapes;
using System;
//using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using IoC.Configuration;
using WinUI3Demo.Interfaces;
using WinUI3Demo.RandomNumber;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IoC.Configuration.DiContainerBuilder.FileBased;
using IoC.Configuration.DiContainerBuilder;
using OROptimizer;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3Demo;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    //public IServiceProvider Services { get; }

    public static IHost AppHost { get; private set; }

    //public static new App Current => (App)Application.Current;

    /// <summary>
    /// Initializes the singleton application object.
    /// </summary>
    public App()
    {
        //Services = ConfigureServices();
        this.InitializeComponent();

        ConfigureDi();
    }

    private static void ConfigureDi()
    {
        //var hostBuilder = Host.CreateDefaultBuilder();

        // hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory())
        //     .ConfigureServices();

        /*AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register services
                //services.AddSingleton<IMyService, MyService>();
                services.AddSingleton<IRandomNumberGenerator, WinUI3Demo.Extension.RandomNumberGenerator>();

                // Register view models
                services.AddTransient<RandomNumberViewModel>();

                // Register views (optional)
                services.AddTransient<MainWindow>();
            })
            .Build();*/

        var hostBuilder = Host.CreateDefaultBuilder();

        var diContainerBuilder = new IoC.Configuration.DiContainerBuilder.DiContainerBuilder();

        var fileBasedConfigurationParameters = new FileBasedConfigurationParameters(
            new FileBasedConfigurationFileContentsProvider("IoCConfiguration.xml"),
            AppContext.BaseDirectory, new AllLoadedAssemblies())
        {
            AttributeValueTransformers = [new FileFolderPathAttributeValueTransformer()]
        };

        var hostIntegratedContainerInfo = diContainerBuilder.StartFileBasedDi(fileBasedConfigurationParameters)
            .WithoutPresetDiContainer()
            // Add additional modules using AddAdditionalDiModules() one or multiple times as necessary
            // to register modules in addition to DI specified in "IoCConfiguration.xml"
            // If method is not called, only the 
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
        await AppHost.StartAsync();

        m_window = AppHost.Services.GetRequiredService<MainWindow>();
        //m_window = Services.GetRequiredService<MainWindow>();
        m_window.Activate();
    }

    private Window m_window;
}