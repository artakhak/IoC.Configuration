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
using WinUI3Demo.Interfaces;
using WinUI3Demo.RandomNumber;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;


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

    public static new App Current => (App)Application.Current;

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
        var hostBuilder = Host.CreateDefaultBuilder();
        
        // hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory())
        //     .ConfigureServices();
        
        AppHost = Host.CreateDefaultBuilder()
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
            .Build();

    }

    // private static IServiceProvider ConfigureServices_OLD()
    // {
    //     var services = new ServiceCollection();
    //
    //     // Services
    //     services.AddSingleton<IRandomNumberGenerator, RandomNumberGenerator>();
    //
    //     // ViewModels
    //     //services.AddTransient<RandomNumberViewModel>();
    //         
    //     // Register Windows
    //     //services.AddTransient<MainWindow>();
    //
    //     // Register your custom services here, for example:
    //     // services.AddSingleton<IDataService, DataService>();
    //
    //     return services.BuildServiceProvider();
    // }

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

    /*static IFileBasedContainerStarter CreateContainer(WebApplicationBuilder webApplicationBuilder, Func<IMvcBuilder> createControllerBuilder)
    {
        
    }*/
    
    
}