using IoC.Configuration;
using IoC.Configuration.AspNetConfigureHostBuilder;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainerBuilder.FileBased;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using WebApiDemo.Startup;
using LogHelperContextLogToConsole = WebApiDemo.Startup.LogHelperContextLogToConsole;

//using LogHelperContextLogToConsole = OROptimizer.Diagnostics.Log.LogHelperContextLogToConsole;

var builder = WebApplication.CreateBuilder(args);

LogHelper.RegisterContext(new LogHelperContextLogToConsole());

var containerStarter = CreateContainer(builder, () => builder.Services.AddControllers());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var containerInfo = containerStarter.Start();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

static IFileBasedContainerStarter CreateContainer(WebApplicationBuilder webApplicationBuilder, Func<IMvcBuilder> createControllerBuilder)
{
    var diContainerBuilder = new IoC.Configuration.DiContainerBuilder.DiContainerBuilder();

    var fileBasedConfigurationParameters = new FileBasedConfigurationParameters(
        new FileBasedConfigurationFileContentsProvider("IoCConfiguration.xml"),
        AppContext.BaseDirectory, new AllLoadedAssemblies())
    {
        AttributeValueTransformers = new[] { new FileFolderPathAttributeValueTransformer() },
        //ConfigurationFileXmlDocumentLoaded = (sender, e) =>
        //{
            
        //    //Helpers.EnsureConfigurationDirectoryExistsOrThrow(e.XmlDocument.SelectElement("/iocConfiguration/appDataDir").GetAttribute("path"));

        //    //Helpers.ReplaceActiveDiManagerInConfigurationFile(e.XmlDocument, _diImplementationType);
        //    //configurationFileXmlDocumentLoadedEventHandler?.Invoke(e);
        //},
    };

    var containerStarter = diContainerBuilder.StartFileBasedDi(fileBasedConfigurationParameters, out var loadedConfiguration)
        .WithoutPresetDiContainer()
        //.AddAdditionalDiModules(new TestModule2())
        .RegisterModules(new AspNetApplicationHostBuilder(webApplicationBuilder.Host));

    var controllerBuilder = createControllerBuilder(); // webApplicationBuilder.Services.AddControllers();

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

    AddControllersFromWebApi(loadedConfiguration.WebApi);

    if (loadedConfiguration.PluginsSetup != null)
    {
        foreach (var pluginSetup in loadedConfiguration.PluginsSetup.AllPluginSetups.Where(x => x.Enabled))
            AddControllersFromWebApi(pluginSetup.WebApi);
    }
    
    return containerStarter;
}