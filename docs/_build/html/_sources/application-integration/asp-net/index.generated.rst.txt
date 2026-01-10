===========================
ASP.NET Service Integration
===========================

Integration with ASP.NET Core allows you to use IoC.Configuration to manage the lifecycle and resolution of your web services, including API controllers. This is achieved via the ``IoC.Configuration.AspNet`` Nuget library (source code available at `IoC.Configuration.AspNet <https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration.AspNet>`_).

Implementation Details
----------------------

To integrate with ASP.NET Core, you use the ``IoC.Configuration.AspNet`` Nuget library (source code available at `IoC.Configuration.AspNet <https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration.AspNet>`_). This allows the ``IHostBuilder`` (or ``WebApplicationBuilder``) to delegate service resolution to the underlying container configured in your IoC.Configuration XML file (e.g., Autofac).

Key steps include:

- Creating a ``WebApplicationHostBuilder`` wrapper for your ``IHostBuilder`` (provided in the `IoC.Configuration.AspNet <https://www.nuget.org/packages/IoC.Configuration.AspNet>`_ package).
- Calling ``WithHostBuilder`` during the IoC.Configuration startup sequence.

Example
-------

For a complete implementation, refer to the example project ``WebApiDemo.csproj`` and related projects in `ApplicationIntegrationDemos/ASP.NET.WebApi <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/ASP.NET.WebApi>`_:

In this project, you can see how the ``Program.cs`` or ``Startup.cs`` is modified to initialize ``DiContainerBuilder`` and connect it to the web host.
The code snippet below shows how the methods ``WithHostBuilder`` and ``RegisterServiceProviderAndBuildApp`` are used in `WebApiDemo.Program <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/ASP.NET.WebApi/WebApiDemo/Program.cs>`_ to register the DI container configured in IoC.Configuration file `IoCConfiguration.xml <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/ASP.NET.WebApi/WebApiDemo/IoCConfiguration.xml>`_ with the ASP.NET dependency injection setup.

.. note::
    The helper method ``ControllerRegistrationHelpers.RegisterControllers(loadedConfiguration, controllerBuilder)`` available in the `IoC.Configuration.AspNet <https://www.nuget.org/packages/IoC.Configuration.AspNet>`_  nuget package ensures that controllers specified in the assemblies within ``iocConfiguration/webApi``` and ``iocConfiguration/pluginsSetup/pluginSetup/webApi`` elements are correctly loaded.
    
.. note::
    All classes and interfaces injected into controllers will be resolved by IoC.Configuration container loaded from the ``IoC.Configuration.xml`` configuration file.
    
**Program.cs**:
  
.. sourcecode:: csharp

    using IoC.Configuration;
    using IoC.Configuration.AspNet;
    using IoC.Configuration.AspNet.HostBuilder;
    using IoC.Configuration.DiContainerBuilder;
    using IoC.Configuration.DiContainerBuilder.FileBased;
    using OROptimizer;
    using OROptimizer.Diagnostics.Log;
    using WebApiDemo.Startup;

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
            // If the method is not called, only the 
            //.AddAdditionalDiModules(new MyModule())

            // Use WithHostBuilder(hostBuilder) to make sure IoC.Configuration will register DI with the host builder
            // Do not call hostBuilder.Build() since this will be done by IoC.Configuration.
            .WithHostBuilder(new WebApplicationHostBuilder(webApplicationBuilder))
            .RegisterServiceProviderAndBuildApp(() =>
            {
                addWebApiServices(webApplicationBuilder.Services);
            });
        
        return (hostIntegratedContainerInfo.Host, hostIntegratedContainerInfo.ContainerInfo);
    }

    

**File ``IoCConfiguration.xml``** located in the same directory as ``WebApiDemo.csproj``.

.. code-block:: xml

    <?xml version="1.0" encoding="utf-8"?>

    <!--
       The XML configuration file is validated against schema file IoC.Configuration.Schema.7579ADB2-0FBD-4210-A8CA-EE4B4646DB3F.xsd, 
       which can be found in folder IoC.Configuration.Content in output directory. 
       The schema file can also be downloaded from 
       http://oroptimizer.com/ioc.configuration/V2/IoC.Configuration.Schema.7579ADB2-0FBD-4210-A8CA-EE4B4646DB3F.xsd or in source code 
       project in Github.com.
       
       To use Visual Studio code completion based on schema contents, right click Properties on this file in Visual Studio, and in Schemas 
       field pick the schema IoC.Configuration.Schema.7579ADB2-0FBD-4210-A8CA-EE4B4646DB3F.xsd.

       TODO: Modify this section. Before running the application make sure to execute IoC.Configuration\Tests\IoC.Configuration.Tests\PostBuildCommands.bat to copy the dlls into 
       folders specified in this configuration file.
       Also, modify the batch file to copy the Autofac and Ninject assemblies from Nuget packages folder on machine, where the test is run.
    -->

    <iocConfiguration
    	xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
    	xsi:noNamespaceSchemaLocation="http://oroptimizer.com/IoC.Configuration/V2/IoC.Configuration.Schema.7579ADB2-0FBD-4210-A8CA-EE4B4646DB3F.xsd">

    	<!--The application should have write permissions to a path specified in appDataDir. 
        This is where dynamically generated DLLs are saved.-->
    	<!--NOTE: a path should be an absolute path, or should be converted to an absolute path by some implementation of 
    	IoC.Configuration.AttributeValueTransformer.IAttributeValueTransformer. In this example the paths are converted by 
    	WebApiDemo.Startup.FileFolderPathAttributeValueTransformer.-->
    	<appDataDir
    		path="IoCConfigurationFiles/AutogeneratedDlls/IoCConfiguration" />

    	<plugins pluginsDirPath="IoCConfigurationFiles/PluginDlls">

    		<!--        
            Plugin assemblies will be in a folder with similar name under pluginsDirPath folder.
            The plugin folders will be included in an assembly resolution mechanism.        
            -->
    		<!--A folder 'K:\...\IoC.Configuration\ApplicationIntegrationDemos\ASP.NET.WebApi\WebApiDemo\bin\Debug\net8.0\IoCConfigurationFiles\PluginDlls\WebApiDemoExtension' should exist.  -->
    		<plugin name="WebApiDemoExtension" />
    	</plugins>

    	<additionalAssemblyProbingPaths>
    		<probingPath
    			path="IoCConfigurationFiles/DynamicallyLoadedDlls" />
    		<probingPath
    			path="IoCConfigurationFiles/ThirdPartyLibs" />
    		<probingPath
    			path="IoCConfigurationFiles/ContainerImplementations/Autofac" />
    		<probingPath
    			path="IoCConfigurationFiles/ContainerImplementations/Ninject" />
    	</additionalAssemblyProbingPaths>

    	<assemblies>
    		<!--Assemblies should be in one of the following locations:
            1) Executable's folder
            2) In folder specified in additionalAssemblyProbingPaths element.
            3) In one of the plugin folders specified in the plugins element (only for assemblies with plugin attribute) -->
    		<assembly name="OROptimizer.Shared" alias="oroptimizer_shared" />
    		<assembly name="IoC.Configuration" alias="ioc_config" />
    		<assembly name="IoC.Configuration.Autofac" alias="autofac_ext" />
    		<assembly name="IoC.Configuration.Ninject" alias="ninject_ext" />
    		<assembly name="WebApiDemo" alias="web_api_demo" />
    		<assembly name="WebApiDemo.DynamicallyLoadedControllers" alias="web_api_demo_dynamic_1" />
    		<assembly name="WebApiDemo.Extension" alias="web_api_demo_ext" plugin="WebApiDemoExtension" />		
    	</assemblies>

    	<typeDefinitions>
    		
    	</typeDefinitions>

    	<!--an assembly attribute is not required, and only is needed to make sure the type is looked at a specific assembly 
        If the assembly attribute is omitted, the type will be looked in all assemblies specified in assemblies, plus some additional 
        assemblies such as OROptimizer.Shared, IoC.Configuration, etc.
    	-->
    	<parameterSerializers serializerAggregatorType="OROptimizer.Serializer.TypeBasedSimpleSerializerAggregator"
    						  assembly="oroptimizer_shared">
    		<!--
            Use parameters element to specify constructor parameters, if the type specified in 'serializerAggregatorType' attribute
            has non-default constructor.
            -->
    		<!--<parameters>
            </parameters>-->
    		<serializers>
    			<parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerDouble" />
    			<parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerLong" />
    			<parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerInt"/>
    			<parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerShort"/>
    			<parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerByte" />
    			<parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerBoolean" />
    			<parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerDateTime" />
    			<parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerString" />			
    		</serializers>

    	</parameterSerializers>

    	<!--The value of the type attribute should be a type that implements IoC.Configuration.DiContainer.IDiManager-->
    	<diManagers activeDiManagerName="Autofac">		
    		<diManager name="Autofac" type="IoC.Configuration.Autofac.AutofacDiManager">
    		</diManager>

    		<!--
            ASP.NET DI integration is not yet supported IoC.Configuration.Ninject library.
            Might be supported later if Ninject library implements the Microsoft.Extensions.DependencyInjection.IServiceProviderFactory
            -->
    		<!--<diManager name="Ninject" type="IoC.Configuration.Ninject.NinjectDiManager">
    		</diManager>-->
    	</diManagers>

    	<!--
        If settingsRequestor element is used, the type in the type attribute should 
        specify a type that implements IoC.Configuration.ISettingsRequestor. 
        The implementation specifies a collection of required settings that should be present
        in a settings element.
        Note, the type specified in the type attribute is fully integrated into a dependency 
        injection framework. In other words, constructor parameters will be injected using 
        bindings specified in the dependencyInjection element.
        -->
    	<!--<settingsRequestor type="SharedServices.FakeSettingsRequestor">
    	</settingsRequestor>-->

    	<settings>
    	</settings>

    	<!--
          webApi is an optional element that contains ASP.NET Core related 
          sections such as assemblies with API controllers, etc.
        -->
    	<webApi>
    		<controllerAssemblies>
    			<!--
                	Specify assemblies with API controllers.
                	No need to include 'controllerAssembly' for an assembly where the Program class is 
                	since controllers in that assembly will be automatically included 
                -->
    			<controllerAssembly assembly="web_api_demo_dynamic_1" />
    		</controllerAssemblies>
    	</webApi>

    	<dependencyInjection>
    		<modules>
    			
    		</modules>
    		<services>			
    			<service type="WebApiDemo.Domain.Employee.IEmployeeRepository" >
    				<implementation type="WebApiDemo.Domain.Employee.EmployeeRepository" scope="singleton">					
    				</implementation>
    			</service>
    			<service type="WebApiDemo.DynamicallyLoadedControllers.Controllers.IRandomNumberGenerator">
    				<implementation type="WebApiDemo.DynamicallyLoadedControllers.Controllers.RandomNumberGenerator" scope="singleton">					
    				</implementation>
    			</service>		
    		</services>

    		<autoGeneratedServices>
    			
    		</autoGeneratedServices>
    	</dependencyInjection>

    	<startupActions>		
    		<!--<startupAction type="DynamicallyLoadedAssembly1.Implementations.StartupAction2">
    		</startupAction>-->
    	</startupActions>

    	<pluginsSetup>
    		<pluginSetup plugin="WebApiDemoExtension">
    			<!--The type in pluginImplementation should be a non-abstract class  
                    that implements IoC.Configuration.IPlugin and which has a public constructor-->
    			<pluginImplementation type="WebApiDemo.Extension.IoCConfigurationSetup.WebApiDemoPlugin">				
    			</pluginImplementation>
    			<settings>				
    			</settings>

    			<webApi>
    				<controllerAssemblies>
    					<!--
                          Specify assemblies with API controllers.
                        -->					
    					<controllerAssembly assembly="web_api_demo_ext"/>
    				</controllerAssemblies>
    			</webApi>
    			<dependencyInjection>
    				<modules>
    					
    				</modules>
    				<services>
    					<service type="WebApiDemo.Extension.Domain.Company.ICompanyRepository" >
    						<implementation scope="singleton" type="WebApiDemo.Extension.Domain.Company.CompanyRepository">
    						</implementation>
    					</service>
    					
    				</services>
    				<autoGeneratedServices>
    					
    				</autoGeneratedServices>
    			</dependencyInjection>
    		</pluginSetup>		
    	</pluginsSetup>
    </iocConfiguration>

