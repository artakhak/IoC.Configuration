===============================
WinUI 3 Application Integration
===============================

WinUI 3 applications can leverage IoC.Configuration to manage complex dependency graphs and plugin architectures. Unlike ASP.NET or MAUI, WinUI 3 integration typically uses the default host builder implementations provided in the core library.

How it works
------------

By using the host integration features, WinUI 3 applications can resolve their main windows, services, and navigation components through the IoC.Configuration container. This is achieved by wrapping the standard ``Microsoft.Extensions.Hosting.IHostBuilder`` with the `IoC.Configuration.DiContainerBuilder.ApplicationHostBuilder <https://github.com/artakhak/IoC.Configuration/blob/master/IoC.Configuration/DiContainerBuilder/ApplicationHostBuilder.cs>`_.

Key steps include:

- Initializing the standard .NET ``HostBuilder``.
- Wrapping it in an ``ApplicationHostBuilder``.
- Calling ``WithHostBuilder`` during the IoC.Configuration startup sequence to delegate service resolution.

Example
-------

For a complete implementation, refer to the example project ``WinUI3Demo.csproj`` and related projects in `ApplicationIntegrationDemos/WinUI3 <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/WinUI3>`_:

The code snippet below shows how the methods ``WithHostBuilder`` and ``RegisterServiceProviderAndBuildApp`` are used in `WinUI3Demo.App <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/WinUI3/WinUI3Demo/App.xaml.cs>`_ to register the DI container configured in the IoC.Configuration file `IoCConfiguration.xml <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/WinUI3/WinUI3Demo/IoCConfiguration.xml>`_ with the WinUI 3 application host.

.. note::
    This setup allows WinUI 3 components like the ``MainWindow`` to have their dependencies automatically injected by the IoC.Configuration container.

**App.xaml.cs**:

.. sourcecode:: csharp

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



**File ``IoCConfiguration.xml``** located in the same directory as ``WinUI3Demo.csproj``.

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
    	WinUI3Demo.FileFolderPathAttributeValueTransformer.-->
    	<appDataDir
    		path="IoCConfigurationFiles/AutogeneratedDlls/IoCConfiguration" />

    	<plugins pluginsDirPath="IoCConfigurationFiles/PluginDlls">

    		<!--        
            Plugin assemblies will be in a folder with similar name under pluginsDirPath folder.
            The plugin folders will be included in an assembly resolution mechanism.        
            -->
    		<!--A folder 'K:\...\IoC.Configuration\ApplicationIntegrationDemos\WinUI3\WinUI3Demo\bin\IoCConfigurationFiles\PluginDlls\WinUI3DemoExtension' should exist.  -->
    		<plugin name="WinUI3DemoExtension" />
    	</plugins>

    	<additionalAssemblyProbingPaths>
    		<probingPath
    			path="IoCConfigurationFiles/DynamicallyLoadedDlls" />
    		<probingPath
    			path="IoCConfigurationFiles/ThirdPartyLibs" />
    		<probingPath
    			path="IoCConfigurationFiles/ContainerImplementations/Autofac" />
    	</additionalAssemblyProbingPaths>

    	<assemblies>		
    		<!--Assemblies should be in one of the following locations:
            1) Executable's folder
            2) In folder specified in additionalAssemblyProbingPaths element.
            3) In one of the plugin folders specified in the plugins element (only for assemblies with plugin attribute) -->
    		<assembly name="OROptimizer.Shared" alias="oroptimizer_shared" />
    		<assembly name="IoC.Configuration" alias="ioc_config" />
    		<assembly name="IoC.Configuration.Autofac" alias="autofac_ext" />
    		<assembly name="WinUI3Demo" alias="win_ui3_demo" />
    		<assembly name="WinUI3Demo.Interfaces" alias="win_ui3_demo_interfaces" />
    		<assembly name="WinUI3Demo.Extension" alias="win_ui3_demo_ext" plugin="WinUI3DemoExtension" />		
    	</assemblies>

    	<typeDefinitions>
    		
    	</typeDefinitions>

    	<!--assembly attribute is not required, and only is needed to make sure the type is looked at a specific assembly 
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

    	<!--The value of the type attribute should be a type that implements 
        IoC.Configuration.DiContainer.IDiManager-->
    	<diManagers activeDiManagerName="Autofac">
    		<diManager name="Autofac" type="IoC.Configuration.Autofac.AutofacDiManager">
    		</diManager>

    		<!--
    		WinUI3 DI integration is not yet supported IoC.Configuration.Ninject library.
    		Might be supported later if Ninject library implements the Microsoft.Extensions.DependencyInjection.IServiceProviderFactory.
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

    	<dependencyInjection>
    		<modules>
    		</modules>
    		
    		<services>
    			
    			<selfBoundService type="WinUI3Demo.MainWindow" scope="transient">				
    			</selfBoundService>
    			<selfBoundService type="WinUI3Demo.RandomNumber.RandomNumberViewModel" scope="transient">
    			</selfBoundService>

    			<!--Bindings for non-plugin interfaces to implementions in plugins.-->
    			<service type="WinUI3Demo.Interfaces.IRandomNumberGenerator" >
    				<implementation type="WinUI3Demo.Extension.RandomNumberGenerator" scope="singleton">
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
    		<pluginSetup plugin="WinUI3DemoExtension">
    			<!--The type in pluginImplementation should be a non-abstract class  
                    that implements IoC.Configuration.IPlugin and which has a public constructor-->
    			<pluginImplementation type="WinUI3Demo.Extension.IoCConfigurationSetup.WinUI3DemoPlugin">				
    			</pluginImplementation>
    			<settings>				
    			</settings>
    			
    			<dependencyInjection>
    				<modules>
    					
    				</modules>
    				<services>
    								
    				</services>
    				<autoGeneratedServices>
    					
    				</autoGeneratedServices>
    			</dependencyInjection>
    		</pluginSetup>		
    	</pluginsSetup>
    </iocConfiguration>
