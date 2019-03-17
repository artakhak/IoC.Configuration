================================
IoCConfiguration_autoService.xml
================================

This configuration file can be downloaded downloaded from `IoCConfiguration_autoService.xml <https://github.com/artakhak/IoC.Configuration/blob/master/IoC.Configuration.Tests/IoCConfiguration_autoService.xml>`_.

.. code-block:: xml
   :linenos:

   <?xml version="1.0" encoding="utf-8"?>

   <!--
      The XML configuration file is validated against schema file IoC.Configuration.Schema.2F7CE7FF-CB22-40B0-9691-EAC689C03A36.xsd,
      which can be found in folder IoC.Configuration.Content in output directory.
      The schema file can also be downloaded from
      http://oroptimizer.com/ioc.configuration/V2/IoC.Configuration.Schema.2F7CE7FF-CB22-40B0-9691-EAC689C03A36.xsd or in source code
      project in Github.com.

      To use Visual Studio code completion based on schema contents, right click Properties on this file in Visual Studio, and in Schemas
      field pick the schema IoC.Configuration.Schema.2F7CE7FF-CB22-40B0-9691-EAC689C03A36.xsd.

      Before running the tests make sure to execute IoC.Configuration\Tests\IoC.Configuration.Tests\PostBuildCommands.bat to copy the dlls into
      folders specified in this configuration file.
      Also, modify the batch file to copy the Autofac and Ninject assemblies from Nuget packages folder on machine, where the test is run.
   -->

   <iocConfiguration>

     <!--The application should have write permissions to path specified in appDataDir.
       This is where dynamically generated DLLs are saved.-->
     <appDataDir
       path="K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\IoC.Configuration.Tests\bin\TestFiles\DynamicFiles_AutoServiceTests" />

     <plugins pluginsDirPath="K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\IoC.Configuration.Tests\TestDlls\PluginDlls">

       <!--
           Plugin assemblies will be in a folder with similar name under pluginsDirPath folder.
           The plugin folders will be included in assembly resolution mechanism.
           -->

       <!--A folder K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\Tests\IoC.Configuration.Tests\TestDlls\PluginDlls\Plugin1 should exist.  -->
       <plugin name="Plugin1" />
       <plugin name="Plugin2" enabled="true" />
       <plugin name="Plugin3" enabled="false" />
     </plugins>

     <additionalAssemblyProbingPaths>
       <probingPath
         path="K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\IoC.Configuration.Tests\TestDlls\ThirdPartyLibs" />
       <probingPath
         path="K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\IoC.Configuration.Tests\TestDlls\ContainerImplementations\Autofac" />
       <probingPath
         path="K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\IoC.Configuration.Tests\TestDlls\ContainerImplementations\Ninject" />
       <probingPath
         path="K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\IoC.Configuration.Tests\TestDlls\DynamicallyLoadedDlls" />
     </additionalAssemblyProbingPaths>

     <assemblies>
       <!--Assemblies should be in one of the following locations:
           1) Executable's folder
           2) In folder specified in additionalAssemblyProbingPaths element.
           3) In one of the plugin folders specified in plugins element (only for assemblies with plugin attribute) -->

       <!--
           Use "loadAlways" attribute to make sure a reference to assembly is added to dynamically generated assembly for
           dependencies, even if the assembly is not referenced anywhere in configuration file.
           In general, this is not necessary, however in case if generating dynamic assembly fails
           because of missing .NET assemblies, using this field might help.
           Use "overrideDirectory" attribute, to make the assembly path explicit, rather then searching for
           an assembly in predefined folders, which also include
           probing paths specified in additionalAssemblyProbingPaths element.
           -->

       <assembly name="IoC.Configuration.Autofac" alias="autofac_ext" />
       <assembly name="IoC.Configuration.Ninject" alias="ninject_ext" />

       <assembly name="TestProjects.Modules" alias="modules" />

       <assembly name="TestProjects.DynamicallyLoadedAssembly1"
                 alias="dynamic1" />

       <assembly name="TestProjects.DynamicallyLoadedAssembly2"
                 alias="dynamic2" />

       <assembly name="TestProjects.TestPluginAssembly1"
                 alias="pluginassm1" plugin="Plugin1" />

       <assembly name="TestProjects.TestPluginAssembly2"
                 alias="pluginassm2" plugin="Plugin2" />

       <assembly name="TestProjects.TestPluginAssembly3"
                 alias="pluginassm3" plugin="Plugin3" />

       <assembly name="TestProjects.ModulesForPlugin1"
                 alias="modules_plugin1" plugin="Plugin1" />

       <assembly name="TestProjects.SharedServices" alias="shared_services" />

       <assembly name="IoC.Configuration.Tests" alias="tests" />
     </assemblies>

     <typeDefinitions>
       <typeDefinition alias="AutoService_IInterface1" type="IoC.Configuration.Tests.AutoService.Services.IInterface1" />
       <typeDefinition alias="IActionValidator" type="SharedServices.Interfaces.IActionValidator" />
       <typeDefinition alias="IProjectGuids" type="IoC.Configuration.Tests.AutoService.Services.IProjectGuids" />
       <typeDefinition alias="ActionTypes" type="SharedServices.DataContracts.ActionTypes" />
       <typeDefinition alias="Guid" type="System.Guid" />
     </typeDefinitions>

     <parameterSerializers>
       <serializers></serializers>
     </parameterSerializers>

     <!--The value of type attribute should be a type that implements
       IoC.Configuration.DiContainer.IDiManager-->
     <diManagers activeDiManagerName="Autofac">
       <diManager name="Ninject" type="IoC.Configuration.Ninject.NinjectDiManager"
                  assembly="ninject_ext">
         <!--
               Use parameters element to specify constructor parameters,
               if the type specified in 'type' attribute has non-default constructor.
               -->
         <!--<parameters>
               </parameters>-->
       </diManager>

       <diManager name="Autofac" type="IoC.Configuration.Autofac.AutofacDiManager"
                  assembly="autofac_ext">
       </diManager>
     </diManagers>

     <!--
       If settingsRequestor element is used, the type in type attribute should
       specify a type that implements IoC.Configuration.ISettingsRequestor.
       The implementation specifies a collection of required settings that should be present
       in settings element.
       Note, the type specified in type attribute is fully integrated into a dependency
       injection framework. In other words, constructor parameters will be injected using
       bindings specified in dependencyInjection element.
       -->

     <settings>
       <constructedValue name="DefaultDBConnection" type="SharedServices.Implementations.SqliteDbConnection">
         <parameters>
           <string name="filePath" value="c:\SQLiteFiles\MySqliteDb.sqlite"/>
         </parameters>
       </constructedValue>

       <object name="Project1Guid" typeRef="Guid" value="EA91B230-3FF8-43FA-978B-3261493D58A3" />
       <object name="Project2Guid" typeRef="Guid" value="9EDC7F1A-6BD6-4277-9015-5A9277218681" />
     </settings>

     <dependencyInjection>
       <modules>
         <module type="IoC.Configuration.Tests.PrimitiveTypeDefaultBindingsModule">
           <parameters>
             <!--Date time can be also long value for ticks. For example the datetime value below can
             be replaced with 604096704000000000-->
             <datetime name="defaultDateTime" value="1915-04-24 00:00:00.000" />
             <double name="defaultDouble" value="0" />
             <int16 name="defaultInt16" value="0" />
             <classMember name="defaultInt32" class="System.Int32" memberName="MinValue"/>
           </parameters>
         </module>

         <module type="IoC.Configuration.Tests.AutoService.AutoServiceTestsModule" />
       </modules>
       <services>

       </services>

       <autoGeneratedServices>

         <!--The scope for autoService implementations is always singleton -->
         <autoService interfaceRef="IProjectGuids" >

           <!--Note, since property Project1 in IoC.Configuration.Tests.AutoService.Services.IProjectGuids has
           a setter, the implementation will implement the setter as well.-->
           <autoProperty name="Project1" returnTypeRef="Guid">
             <object typeRef="Guid" value="966FE6A6-76AC-4895-84B2-47E27E58FD02"/>
           </autoProperty>

           <autoProperty name="Project2" returnTypeRef="Guid">
             <object typeRef="Guid" value="AC4EE351-CE69-4F89-A362-F833489FD9A1"/>
           </autoProperty>

           <autoMethod name="GetDefaultProject" returnTypeRef="Guid">
             <!--No methodSignature is required, since the method does not have any parameters.-->
             <default>
               <!--TODO: change the returned value to classMember which references IProjectGuids.Project1 -->
               <object typeRef="Guid" value="1E08071B-D02C-4830-AE3C-C9E78A29EA37"/>

             </default>
           </autoMethod>

           <!---IoC.Configuration.Tests.AutoService.Services.IProjectGuids also has a method NotImplementedMethod()
           which will be auto-implemented as well.-->
         </autoService>

         <!--Demo of referencing auto-implemented method parameters using parameterValue element-->
         <autoService interface="IoC.Configuration.Tests.AutoService.Services.IAppInfoFactory">
           <autoMethod name="CreateAppInfo" returnType="IoC.Configuration.Tests.AutoService.Services.IAppInfo">
             <methodSignature>
               <int32 paramName="appId"/>
               <string paramName="appDescription"/>
             </methodSignature>
             <default>
               <constructedValue type="IoC.Configuration.Tests.AutoService.Services.AppInfo">
                 <parameters>
                   <!--The value of name attribute is the name of constructor parameter in AppInfo-->
                   <!--
                   The value of paramName attribute is the name of parameter in IAppInfoFactory.CreateAppInfo.
                   This parameter should be present under autoMethod/methodSignature element.
                   -->
                   <!--In this example the values of name and paramName are similar, however they don't
                   have to be.-->
                   <parameterValue name="appId" paramName="appId" />
                   <parameterValue name="appDescription" paramName="appDescription" />
                 </parameters>
               </constructedValue>
             </default>
           </autoMethod>
         </autoService>

         <!--The scope for autoService implementations is always singleton -->
         <autoService interface="IoC.Configuration.Tests.AutoService.Services.IActionValidatorFactory">

           <autoProperty name="DefaultActionValidator" returnType="SharedServices.Interfaces.IActionValidator">
             <injectedObject type="IoC.Configuration.Tests.AutoService.Services.ActionValidatorDefault"/>
           </autoProperty>

           <autoProperty name="PublicProjectId" returnType="System.Guid" >
             <object type="System.Guid" value="95E352DD-5C79-49D0-BD51-D62153570B61"/>
           </autoProperty>

           <autoMethod name="GetValidators"
                       returnType="System.Collections.Generic.IReadOnlyList[SharedServices.Interfaces.IActionValidator]"
                       reuseValue="true">

             <methodSignature>
               <!--paramName attribute is optional, however it makes the auto-implementation more readable. -->

               <object paramName="actionType" typeRef="ActionTypes"/>
               <object paramName="projectGuid" type="System.Guid"/>
             </methodSignature>

             <!--Parameter actionType (parameter1) value: In this example we use class member ViewFilesList (enum value) in enumeration
             SharedServices.DataContracts.ActionTypes. Note, we use alias ActionTypes to reference the enum type declared in typeDefinitions section.
             -->
             <!--Parameter projectGuid (parameter2) value: The string "F79C3F23-C63F-4EB0-A513-7A8772A82B35" will be de-serialized to a System.Guid value,
             using the default OROptimizer.Serializer.TypeBasedSimpleSerializerGuid serializer. More serializers can be provided in section
             parameterSerializers-->
             <if parameter1="_classMember:ActionTypes.ViewFilesList" parameter2="8663708F-C707-47E1-AEDC-2CD9291AD4CB">
               <collection>
                 <constructedValue type="SharedServices.Implementations.ActionValidator3">
                   <parameters>
                     <int32 name="intParam" value="7"/>
                   </parameters>
                 </constructedValue>

                 <!--Constructor of ActionValidatorWithDependencyOnActionValidatorFactory has a parameter of type
                 IoC.Configuration.Tests.AutoService.Services.IActionValidatorFactory. Therefore an instance of auto-generated service  IActionValidatorFactory
                 will be injected.
                 -->
                 <injectedObject type="IoC.Configuration.Tests.AutoService.Services.ActionValidatorWithDependencyOnActionValidatorFactory"/>

                 <constructedValue type=" IoC.Configuration.Tests.AutoService.Services.ActionValidator1" >
                   <parameters>
                     <injectedObject name="param1" typeRef="AutoService_IInterface1" />
                   </parameters>
                   <injectedProperties>
                     <!-- Note, we could have used constructedValue element to inject a constructed value into property
                       ActionValidator1.Property2. However, to keep the example simple, injectedObject was used -->
                     <injectedObject name="Property2" type="IoC.Configuration.Tests.AutoService.Services.IInterface2" />
                   </injectedProperties>
                 </constructedValue>

                 <injectedObject type="TestPluginAssembly1.Implementations.Plugin1ActionValidator"/>

                 <classMember class="IoC.Configuration.Tests.AutoService.Services.StaticAndConstMembers" memberName="ActionValidator1" />

                 <!--Since DefaultActionValidator property in IoC.Configuration.Tests.AutoService.Services.IActionValidatorValuesProvider interface is
                 not static, IoC.Configuration.Tests.AutoService.Services.IActionValidatorValuesProvider will be injected.
                 Therefore, a binding should be setup for this class (or the interface should be auto-implemented
                 using autoService element)
                 -->
                 <classMember class="IoC.Configuration.Tests.AutoService.Services.IActionValidatorValuesProvider"
                              memberName="DefaultActionValidator"/>

                 <!--Since Plugin3 is disabled, Plugin3ActionValidator will be ignored -->
                 <injectedObject type="TestPluginAssembly3.Implementations.Plugin3ActionValidator"/>
               </collection>
             </if>

             <!--Parameter actionType (parameter1) value: In this example we use full class path for
             SharedServices.DataContracts.ActionTypes in parameter1, instead of referencing a type declared in typeDefinitions element.
             -->
             <!--Parameter projectGuid (parameter2) value: In this case we reference the Project1Guid setting value in settings section, instead
             of using a Guid string-->
             <if parameter1="_classMember:ActionTypes.ViewFileContents" parameter2="_settings:Project1Guid">
               <collection>
                 <!--Since IoC.Configuration.Tests.AutoService.Services.ActionValidator1 and SharedServices.Implementations.ActionValidator2 are
                   concrete (non-interface and non-abstract) classes), and have public constructors,
                   self bound service bindings for these classes will be automatically added, if binding for these classes are not specified
                   in configuration file or in some module of type IoC.Configuration.DiContainer.IDiModule -->

                 <injectedObject type="IoC.Configuration.Tests.AutoService.Services.ActionValidator1" />

                 <!--Since GetViewOnlyActionvalidator() method in IoC.Configuration.Tests.AutoService.Services.IActionValidatorValuesProvider
                 interface is not static, IoC.Configuration.Tests.AutoService.Services.IActionValidatorValuesProvider will be injected.
                 Therefore, a binding should be setup for this class (or the interface should be auto-implemented using
                 autoService element).
                 -->
                 <classMember class="IoC.Configuration.Tests.AutoService.Services.IActionValidatorValuesProvider"
                              memberName="GetViewOnlyActionvalidator"/>
               </collection>
             </if>

             <!--Parameter actionType (parameter1) value: In this case we use constant value DefaultActionType declared in
             class IoC.Configuration.Tests.AutoService.Services.StaticAndConstMembers.
             -->
             <!--Parameter projectGuid (parameter2) value: In this case we use the value of property Project1 in
             IoC.Configuration.Tests.AutoService.Services.IProjectGuids. Since the property Project1 is not static,
             class IoC.Configuration.Tests.AutoService.Services.IProjectGuids will be injected.
             -->
             <if parameter1="_classMember:IoC.Configuration.Tests.AutoService.Services.StaticAndConstMembers.DefaultActionType"
                 parameter2="_classMember:IProjectGuids.Project1">
               <collection>
                 <!--Lets assume no validators are needed for this case-->
               </collection>
             </if>

             <!--Parameter actionType (parameter1) value: In this case we use enum value
             SharedServices.DataContracts.ActionTypes.ViewFileContents. We use a shortcut (an alias) ActionTypes to reference a
             reference the class SharedServices.DataContracts.ActionTypes declared in typeDefintions section.
             -->
             <!--Parameter projectGuid (parameter2) value: In this case we use the value returned by a call to static method
             GetDefaultProjectGuid() in class IoC.Configuration.Tests.AutoService.Services.StaticAndConstMembers.
             -->
             <if parameter1="_classMember:ActionTypes.ViewFileContents"
                 parameter2="_classMember:IoC.Configuration.Tests.AutoService.Services.StaticAndConstMembers.GetDefaultProjectGuid">

               <!--Continue here.-->
               <collection>
                 <!--Since IoC.Configuration.Tests.AutoService.Services.ActionValidator1 and SharedServices.Implementations.ActionValidator2 are
                   concrete (non-interface and non-abstract classes), and have public constructors,
                   self bound service bindings for these classes will be automatically added, if binding for these classes
                   are not specified in configuration file or in some module of type IoC.Configuration.DiContainer.IDiModule -->

                 <injectedObject type="SharedServices.Implementations.ActionValidator2" />
                 <injectedObject type="IoC.Configuration.Tests.AutoService.Services.ActionValidator1" />
               </collection>
             </if>

             <!--Note parameter2 references PublicProjectId property in this
             auto-generated IoC.Configuration.Tests.AutoService.Services.IActionValidatorFactory service. -->
             <if parameter1="_classMember:ActionTypes.ViewFilesList"
                 parameter2="_classMember:IoC.Configuration.Tests.AutoService.Services.IActionValidatorFactory.PublicProjectId">
               <collection>
                 <!--Note, we can reference a property in this auto-generated
                 IoC.Configuration.Tests.AutoService.Services.IActionValidatorFactory service.-->
                 <classMember class="IoC.Configuration.Tests.AutoService.Services.IActionValidatorFactory" memberName="DefaultActionValidator"/>
               </collection>

             </if>
             <!--if none of conditions above are true, the default value will be returned by interface implementation.-->

             <default>
               <collection>
                 <!--We can also call a method or property in auto-generated interface, or in one of its base interfaces.-->
                 <classMember class="IoC.Configuration.Tests.AutoService.Services.IActionValidatorFactory" memberName="DefaultActionValidator"/>
                 <injectedObject type="SharedServices.Implementations.ActionValidator3" />
                 <injectedObject type="DynamicallyLoadedAssembly2.ActionValidator4"/>
               </collection>
             </default>
           </autoMethod>

           <!--Overloaded method GetValidators uses parameters of types System.Int2 and System.string, instead of
           SharedServices.DataContracts.ActionTypes and System.Guid, as in case above.-->
           <autoMethod name="GetValidators"
                       returnType="System.Collections.Generic.IReadOnlyList[SharedServices.Interfaces.IActionValidator]">
             <methodSignature>
               <!--paramName attribute is optional, however it makes the auto-implementation more readable. -->
               <int32 paramName="actionTypeId"/>
               <string paramName="projectGuid" />
             </methodSignature>

             <!-- Attributes parameter1 and parameter2 map values of parameters param1 and param2 in GetInstances() method to returned values. -->
             <if parameter1="0" parameter2="8663708F-C707-47E1-AEDC-2CD9291AD4CB">
               <collection>
                 <injectedObject type="SharedServices.Implementations.ActionValidator3" />
                 <injectedObject type="IoC.Configuration.Tests.AutoService.Services.ActionValidator4" />
               </collection>
             </if>

             <default>
               <collection>
                 <!--We can also call a method or property in auto-generated interface, or in one of its base interfaces.-->
                 <classMember class="IoC.Configuration.Tests.AutoService.Services.IActionValidatorFactory"
                              memberName="DefaultActionValidator"/>
                 <injectedObject type="SharedServices.Implementations.ActionValidator3" />
                 <classMember class="IoC.Configuration.Tests.AutoService.Services.StaticAndConstMembers"
                              memberName="GetDefaultActionValidator" />
                 <classMember class="IoC.Configuration.Tests.AutoService.Services.IActionValidatorValuesProvider"
                              memberName="AdminLevelActionValidator"/>
               </collection>
             </default>
           </autoMethod>

           <!--Note, interface IoC.Configuration.Tests.AutoService.Services.IActionValidatorFactory also has a method
           void SomeMethodThatWillNotBeImplemented(int param1, string param2) and a property int SomeUnImplementedProperty { get; },'
           we chose not to implement in configuration file. Unimplemented methods and properties will be auto-implemented to return default values,
           based on return type defaults.
           -->
         </autoService>

         <!--IMemberAmbiguityDemo demonstrates cases when there are multiple occurrences
         of auto-generated methods and properties with same signatures and return types
         in IMemberAmbiguityDemo and its base interfaces.
         -->
         <autoService interface="IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo">
           <!--GetIntValues(): IReadOnlyList<int> GetIntValues(int param1, string param2)-->
           <autoMethod name="GetIntValues" returnType="System.Collections.Generic.IReadOnlyList[System.Int32]" >
             <methodSignature>
               <int32 paramName="param1"/>
               <string paramName="param2"/>
             </methodSignature>
             <if parameter1="1" parameter2="str1">
               <collection>
                 <int32 value="17"/>
               </collection>
             </if>
             <default>
               <collection>
                 <int32 value="18"/>
                 <int32 value="19"/>
               </collection>
             </default>
           </autoMethod>

           <!--
           This method is declared in IMemberAmbiguityDemo_Parent3, which is a base interface for IMemberAmbiguityDemo.
           We can provide implementation for this interface, even though it has a similar signature and return type as the method
           method IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo.GetIntValues.
           By using the attribute 'declaringInterface', we make a distinction between these two.
           -->
           <autoMethod name="GetIntValues" returnType="System.Collections.Generic.IReadOnlyList[System.Int32]"
                       declaringInterface="IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent3">
             <methodSignature>
               <int32 paramName="param1"/>
               <string paramName="param2"/>
             </methodSignature>
             <default>
               <collection>
                 <int32 value="3"/>
               </collection>
             </default>
           </autoMethod>

           <!---
           The method GetDbConnection(System.Guid appGuid) that return IDbConnection is in two base interfaces
           of IMemberAmbiguityDemo: in IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent1 and in
           IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent2.
           Therefore, to avoid ambiguity, we have to specify the declaring interface in attribute 'declaringInterface'.
           We can specify an implementation for IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent2.GetDbConnection(),
           and IoC.Configuration will generate a similar auto-implementation for the similar method in IMemberAmbiguityDemo_Parent1
           as well.
           -->
           <autoMethod name="GetDbConnection" returnType="SharedServices.Interfaces.IDbConnection"
                       declaringInterface="IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent2">
             <methodSignature>
               <object paramName="appGuid" type="System.Guid"/>
             </methodSignature>
             <default>
               <constructedValue type="SharedServices.Implementations.SqliteDbConnection">
                 <parameters>
                   <string name="filePath" value="c:\mySqliteDatabase.sqlite"/>
                 </parameters>
               </constructedValue>
             </default>
           </autoMethod>

           <!--
           Both IMemberAmbiguityDemo_Parent1 and IMemberAmbiguityDemo_Parent2 have properties called DefaultDbConnection
           with the same return types. We can auto-implement this property for each of these interfaces by using
           declaringInterface attribute in autoProperty element to explicitly specify the interface that own
           the property (declaringInterface can be used in autoMethod as well as demonstrated above)
           -->
           <!--Auto-implementation of IMemberAmbiguityDemo_Parent1.DefaultDbConnection-->
           <autoProperty name="DefaultDbConnection" returnType="SharedServices.Interfaces.IDbConnection"
                         declaringInterface="IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent1">
             <constructedValue type="SharedServices.Implementations.SqliteDbConnection">
               <parameters>
                 <string name="filePath" value="c:\IMemberAmbiguityDemo_Parent1_Db.sqlite"/>
               </parameters>
             </constructedValue>
           </autoProperty>

           <!--Auto-implementation of IMemberAmbiguityDemo_Parent2.DefaultDbConnection-->
           <autoProperty name="DefaultDbConnection" returnType="SharedServices.Interfaces.IDbConnection"
                         declaringInterface="IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent2">
             <constructedValue type="SharedServices.Implementations.SqliteDbConnection">
               <parameters>
                 <string name="filePath" value="c:\IMemberAmbiguityDemo_Parent2_Db.sqlite"/>
               </parameters>
             </constructedValue>
           </autoProperty>

           <!--
           Method GetNumericValue() occurs in both IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent2
           and IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent1_Parent. However, since the return types
           are different (System.Double in IMemberAmbiguityDemo_Parent2, and System.Int32 in IMemberAmbiguityDemo_Parent1_Parent),
           we can auto-implement both them, without using attribute 'declaringInterface' to separate these two implementation.
           -->
           <!--IMemberAmbiguityDemo_Parent2.GetNumericValue() with return type of System.Double-->
           <autoMethod name="GetNumericValue" returnType="System.Double" >
             <default>
               <double value="17.3"/>
             </default>
           </autoMethod>

           <!--IMemberAmbiguityDemo_Parent1_Parent.GetNumericValue() with return type of System.Int32-->
           <autoMethod name="GetNumericValue" returnType="System.Int32" >
             <default>
               <int32 value="19"/>
             </default>
           </autoMethod>

           <!--
           Property NumericValue occurs in both IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent1
           and IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo_Parent2. However, since the return types
           are different (System.Double in IMemberAmbiguityDemo_Parent1, and System.Int32 in IMemberAmbiguityDemo_Parent2),
           we can auto-implement both them, without using attribute 'declaringInterface' to separate these two implementation.
           -->
           <!--IMemberAmbiguityDemo_Parent1.NumericValue with return type of System.Double-->
           <autoProperty name="NumericValue" returnType="System.Double" >
             <double value="18.2"/>
           </autoProperty>

           <!--IMemberAmbiguityDemo_Parent2.NumericValue with return type of System.Int32-->
           <autoProperty name="NumericValue" returnType="System.Int32" >
             <int32 value="14"/>
           </autoProperty>

           <!---Auto-implementing Method with optional parameters:
             int MethodWithOptionalParameters(int param1, double param2 = 3.5, int param3=7); -->
           <autoMethod name="MethodWithOptionalParameters" returnType="System.Int32">
             <methodSignature>
               <int32 paramName="param1"/>
               <double paramName="param2"/>
               <int32 paramName="param3"/>
             </methodSignature>
             <if parameter1="3" parameter2="3.5" parameter3="7">
               <int32 value="17"/>
             </if>
             <default>
               <int32 value="18"/>
             </default>
           </autoMethod>
         </autoService>
       </autoGeneratedServices>
     </dependencyInjection>

     <startupActions>

     </startupActions>

     <pluginsSetup>
       <pluginSetup plugin="Plugin1">
         <!--The type in pluginImplementation should be non-abstract class
                   that implements IoC.Configuration.IPlugin and which has a public constructor-->
         <pluginImplementation type="TestPluginAssembly1.Implementations.Plugin1_Simple">
         </pluginImplementation>

         <settings>
           <int32 name="Int32Setting1" value="10"/>
           <string name="StringSetting1" value="Some text"/>
         </settings>

         <dependencyInjection>
           <modules>

           </modules>
           <services>

           </services>

           <autoGeneratedServices>
             <autoService interface="TestPluginAssembly1.Interfaces.IResourceAccessValidatorFactory">
               <autoMethod name="GetValidators"
                           returnType="System.Collections.Generic.IEnumerable[TestPluginAssembly1.Interfaces.IResourceAccessValidator]"
                           reuseValue="true" >
                 <methodSignature>
                   <string paramName="resourceName"/>
                 </methodSignature>
                 <if parameter1="public_pages">
                   <collection>
                     <injectedObject type="TestPluginAssembly1.Interfaces.ResourceAccessValidator1"/>
                   </collection>

                 </if>
                 <if parameter1="admin_pages">
                   <collection>
                     <injectedObject type="TestPluginAssembly1.Interfaces.ResourceAccessValidator1"/>
                     <injectedObject type="TestPluginAssembly1.Interfaces.ResourceAccessValidator2"/>
                   </collection>
                 </if>
                 <default>
                   <collection>
                     <injectedObject type="TestPluginAssembly1.Interfaces.ResourceAccessValidator2"/>
                     <injectedObject type="TestPluginAssembly1.Interfaces.ResourceAccessValidator1"/>
                   </collection>
                 </default>
               </autoMethod>
             </autoService>
           </autoGeneratedServices>
         </dependencyInjection>
       </pluginSetup>

       <pluginSetup plugin="Plugin2">
         <pluginImplementation type="TestPluginAssembly2.Implementations.Plugin2" assembly="pluginassm2">
           <parameters>
             <boolean name="param1" value="true" />
             <double name="param2" value="25.3" />
             <string name="param3" value="String value" />
           </parameters>
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

       <pluginSetup plugin="Plugin3">
         <pluginImplementation type="TestPluginAssembly3.Implementations.Plugin3" assembly="pluginassm3">

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