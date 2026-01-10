=================================
.NET Maui Application Integration
=================================

IoC.Configuration provides specialized support for .NET MAUI applications via the ``IoC.Configuration.Maui`` Nuget library (source code available at `IoC.Configuration.Maui <https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration.Maui>`_).

Integration Process
-------------------

MAUI applications use a ``MauiAppBuilder`` to configure services. IoC.Configuration hooks into this builder using the ``IIntegratesWithHostBuilder`` interface. This ensures that when MAUI attempts to resolve Pages or ViewModels, it uses the IoC.Configuration container.

Key steps include:

- Creating a ``MauiHostBuilder`` wrapper for your ``MauiAppBuilder`` (provided in the `IoC.Configuration.Maui <https://www.nuget.org/packages/IoC.Configuration.Maui>`_ package).
- Calling ``WithHostBuilder`` during the IoC.Configuration startup sequence.

Example
-------

For a complete implementation, refer to the example project ``MauiDemo.csproj`` and related projects in `ApplicationIntegrationDemos/Maui <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/Maui>`_:

The code snippet below shows how the methods ``WithHostBuilder`` and ``RegisterServiceProviderAndBuildApp`` are used in `MauiDemo.MauiProgram <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/Maui/MauiDemo/MauiProgram.cs>`_ to register the DI container configured in the IoC.Configuration file `IoCConfiguration.xml <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/Maui/MauiDemo/IoCConfiguration.xml>`_ with the MAUI application builder.

.. note::
    By using this integration, all MAUI components (Pages, ViewModels, etc.) registered in the ``MauiAppBuilder`` or in the IoC.Configuration XML file will be resolved through the IoC.Configuration container.

**MauiProgram.cs**:

.. sourcecode:: csharp

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




**File ``IoCConfiguration.xml``** located in the same directory as ``MauiDemo.csproj``.

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
    	MauiDemo.FileFolderPathAttributeValueTransformer.-->
    	<appDataDir
    		path="IoCConfigurationFiles/AutogeneratedDlls/IoCConfiguration" />

    	<plugins pluginsDirPath="IoCConfigurationFiles/PluginDlls">

    		<!--        
            Plugin assemblies will be in a folder with similar name under pluginsDirPath folder.
            The plugin folders will be included in an assembly resolution mechanism.        
            -->
    		<!--A folder 'K:\...\IoC.Configuration\ApplicationIntegrationDemos\Maui\MauiDemo\bin\IoCConfigurationFiles\PluginDlls\MauiDemoExtension' should exist.  -->
    		<plugin name="MauiDemoExtension" />
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
    		<assembly name="MauiDemo" alias="win_ui3_demo" />
    		<assembly name="MauiDemo.Interfaces" alias="win_ui3_demo_interfaces" />
    		<assembly name="MauiDemo.Extension" alias="win_ui3_demo_ext" plugin="MauiDemoExtension" />		
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
    		MAUI DI integration is not yet supported IoC.Configuration.Ninject library.
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
    			
    			<selfBoundService type="MauiDemo.RandomNumber.RandomNumberPage" scope="transient">				
    			</selfBoundService>
    			
    			<selfBoundService type="MauiDemo.RandomNumber.RandomNumberViewModel" scope="transient">
    			</selfBoundService>

    			<!--Bindings for non-plugin interfaces to implementations in plugins.-->
    			<service type="MauiDemo.Interfaces.IRandomNumberGenerator" >
    				<implementation type="MauiDemo.Extension.RandomNumberGenerator" scope="singleton">
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
    		<pluginSetup plugin="MauiDemoExtension">
    			<!--The type in pluginImplementation should be a non-abstract class  
                    that implements IoC.Configuration.IPlugin and which has a public constructor-->
    			<pluginImplementation type="MauiDemo.Extension.IoCConfigurationSetup.MauiDemoPlugin">				
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
