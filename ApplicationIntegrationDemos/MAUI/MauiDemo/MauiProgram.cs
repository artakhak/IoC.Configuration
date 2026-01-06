using IoC.Configuration;
using IoC.Configuration.DiContainerBuilder;
using IoC.Configuration.DiContainerBuilder.FileBased;
using MauiDemo.Interfaces;
using MauiDemo.RandomNumber;
using Microsoft.Extensions.Hosting;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;

namespace MauiDemo;

public static class MauiProgram
{
    /*public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}*/
    
    public static MauiApp CreateMauiApp()
    {
        //OROptimizer.Serializer.TypeBasedSimpleSerializerAggregator test = new TypeBasedSimpleSerializerAggregator();

        LogHelper.RegisterContext(new LogHelperContextLogToConsole());

        var builder = MauiApp.CreateBuilder();
        //builder.UseMauiApp<App>();

        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        var (mauiApp, containerInfo) = ConfigureDiAndBuildMauiApp(builder);

        var test4 = containerInfo.DiContainer.Resolve<IRandomNumberGenerator>();
        var test3 = mauiApp.Services.GetRequiredService<IRandomNumberGenerator>();

        var test2 = containerInfo.DiContainer.Resolve<RandomNumberViewModel>();
        var test = mauiApp.Services.GetRequiredService<RandomNumberViewModel>();
        return mauiApp;
    }

    private static (MauiApp mauiApp, IContainerInfo) ConfigureDiAndBuildMauiApp(MauiAppBuilder mauiAppBuilder)
    {
        //var hostBuilder = Host.CreateDefaultBuilder();

        var diContainerBuilder = new DiContainerBuilder();

        var fileBasedConfigurationParameters = new FileBasedConfigurationParameters(
            new FileBasedConfigurationFileContentsProvider(MauiDemo.Properties.Settings.Default.IoCConfigurationFilePath),
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

        //var diContainer = hostIntegratedContainerInfo.ContainerInfo.DiContainer;
        // From this point on either AppHost.Services or hostIntegratedContainerInfo.ContainerInfo.DiContainer
        // can be used to resolve services. Both will use the same DI container.

        return (hostIntegratedContainerInfo.Host.MauiApp, hostIntegratedContainerInfo.ContainerInfo);
    }
}

public class MauiAppHost : IHost
{
    public MauiAppHost(MauiApp mauiApp)
    {
        MauiApp = mauiApp;
    }

    public MauiApp MauiApp { get; }

    public void Dispose()
    {
        MauiApp.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public IServiceProvider Services => MauiApp.Services;
}

public class MauiHostBuilder : IApplicationHostBuilder<MauiAppHost>
{
    private readonly MauiAppBuilder _mauiAppBuilder;
    //private Autofac.ContainerBuilder? _containerBuilder;

    private object? _factory = null;

    public MauiHostBuilder(MauiAppBuilder mauiAppBuilder)
    {
        _mauiAppBuilder = mauiAppBuilder;
    }

    public MauiAppHost Build()
    {
        var mauiAppHost = new MauiAppHost(_mauiAppBuilder.Build());

        // Call _containerBuilder.Build()
        //_containerBuilder!.Build();
        return mauiAppHost;
    }

    public void UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : class
    {
        /*_mauiAppBuilder.ConfigureContainer(factory, containerBuilder =>
        {
            _factory = factory;
            _containerBuilder = containerBuilder as Autofac.ContainerBuilder;
        });*/
        
        _mauiAppBuilder.ConfigureContainer(factory);
        _factory = factory;
    }

    public void ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _mauiAppBuilder.ConfigureContainer<TContainerBuilder>((IServiceProviderFactory<TContainerBuilder>)_factory, (  builder) => 
            configureDelegate(null!, builder));
    }
}

