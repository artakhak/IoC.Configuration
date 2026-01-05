using IoC.Configuration;
using IoC.Configuration.AspNet;

using IoC.Configuration.DiContainerBuilder;
using IoC.Configuration.DiContainerBuilder.FileBased;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using WebApiDemo.Startup;
using LogHelperContextLogToConsole = WebApiDemo.Startup.LogHelperContextLogToConsole;

LogHelper.RegisterContext(new LogHelperContextLogToConsole());

var builder = WebApplication.CreateBuilder(args);

var appData = CreateContainer(builder, () => builder.Services.AddControllers(),
    (addWebApiServices) =>
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        addWebApiServices.AddEndpointsApiExplorer();
        addWebApiServices.AddSwaggerGen();
    });

var app = appData.webApplication;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

static (WebApplication webApplication, IContainerInfo containerInfo) CreateContainer(WebApplicationBuilder webApplicationBuilder, 
    Func<IMvcBuilder> createControllerBuilder,
    Action<IServiceCollection> addWebApiServices)
{
    var diContainerBuilder = new DiContainerBuilder();

    var fileBasedConfigurationParameters = new FileBasedConfigurationParameters(
        new FileBasedConfigurationFileContentsProvider("IoCConfiguration.xml"),
        AppContext.BaseDirectory, new AllLoadedAssemblies())
    {
        AttributeValueTransformers = [new FileFolderPathAttributeValueTransformer()]
    };

    var hostIntegratedContainerInfo = diContainerBuilder.StartFileBasedDi(fileBasedConfigurationParameters,
        loadedConfiguration =>
        {
            var controllerBuilder = createControllerBuilder();
            ControllerRegistrationHelpers.RegisterControllers(loadedConfiguration, controllerBuilder);
        })
        .WithoutPresetDiContainer()
        // Add additional modules using AddAdditionalDiModules() one or multiple times as necessary
        // to register modules in addition to DI specified in "IoCConfiguration.xml"
        // If method is not called, only the 
        //.AddAdditionalDiModules(new MyModule())

        // Use WithHostBuilder(hostBuilder) to make sure IoC.Configuration will register DI with the host builder
        // Do not call hostBuilder.Build() since this will be done by IoC.Configuration.
        .WithHostBuilder(new WebApplicationHostBuilder(webApplicationBuilder))

        // TODO: Call addWebApiServices() before the application is built.
        .RegisterServiceProviderAndBuildApp(() =>
        {
            addWebApiServices(webApplicationBuilder.Services);
        });
    
    return (hostIntegratedContainerInfo.Host, hostIntegratedContainerInfo.ContainerInfo);
}