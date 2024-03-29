=======================
**classMember** element
=======================

The **classMember** element is used to reference class member values (i.e., class variables, constant values, properties, method call results) in configuration file.

This element uses attribute **class** (and optional attribute **assembly**), or alternatively, an attribute **classRef** to specify the class, as well as attribute **memberName**, to specify the class member name.

The element **classMember** can be used to reference enum values as well. Example: <classMember class="SharedServices.DataContracts.ActionTypes" memberName="ViewFilesList" />.

Referencing non-static and non-constant class members
=====================================================

If the class member is non-static, and non-constant, **IoC.Configuration** will get the class member value by first resolving the class instance from the dependency injection container.

If the class is non-interface, non-abstract, and has a public constructor, **IoC.Configuration** will generate a self-binding for the class.

Otherwise, a binding should be provided either in configuration file, or in one of dependency injection modules.

.. note::
    Refer to :doc:`../../sample-files/IoCConfiguration_classMember.generated` for more examples on **classMember** element.

Example 1: Using **classMember** to provide a service implementation
====================================================================

.. code-block:: xml
    :linenos:

    <service type="System.Int32">
        <valueImplementation scope="singleton">
          <!--Example of classMember in valueImplementation.
          Since IAppIds.DefaultAppId is non-static,
          IAppIds will be resolved from dependency injection container, and the
          value of property DefaultAppId of resolved object will be bound
          to System.Int32
          -->
            <classMember classRef="IAppIds" memberName="DefaultAppId" />
        </valueImplementation>
    </service>

Example 2: Using **classMember** in **collection** element
==========================================================

.. code-block:: xml
    :linenos:

    <service type="System.Collections.Generic.IReadOnlyList[System.Int32]" >
        <valueImplementation scope="singleton">
            <collection>
                <!--Demo of classMember in collection element.-->
                <classMember classRef="ConstAndStaticAppIds" memberName="AppId1"/>
                <classMember classRef="IAppIds" memberName="DefaultAppId"/>
            </collection>
        </valueImplementation>
    </service>

Example 3: Using **classMember** to specify a returned value in **autoProperty** element
========================================================================================

.. code-block:: xml
    :linenos:

    <autoGeneratedServices>

        <!--The scope for autoService implementations is always singleton -->
        <autoService interfaceRef="IAppIds">
            <autoProperty name="DefaultAppId" returnType="System.Int32">
                <!--Example of using classMember attribute in auto property.-->
                <classMember class="System.Int32" memberName="MaxValue"/>
            </autoProperty>
        </autoService>

Example 3: Referencing class member in **if** element under **autoMethod** element
==================================================================================

To reference class members in **if** element attributes in **autoMethod**, use **_classMember:** prefix followed by class full name (or type alias name, for a type declared in **typeDefinition** element), period, and the class member name.

.. note::
    Refer to :doc:`../autogenerated-services/index` and :doc:`../autogenerated-services/auto-method` for more details on **autoMethod** element.

In the example below, we reference a class member **IoC.Configuration.Tests.ClassMember.Services.IAppIds.DefaultAppId** (it is assumed that the configuration has a **typeDefinition** element for a type **IoC.Configuration.Tests.ClassMember.Services.IAppIds**, that has an alias **IAppIds**)

.. code-block:: xml
    :linenos:

    <autoService interface="IoC.Configuration.Tests.ClassMember.Services.IAppIdToPriority">
        <autoMethod name="GetPriority" returnType="System.Int32">
            <methodSignature>
                <int32 paramName="appId"/>
            </methodSignature>

            <!--Property IoC.Configuration.Tests.ClassMember.Services.IAppIds.DefaultAppId
                is non-static, therefore IoC.Configuration.Tests.ClassMember.Services.IAppIds
                will be resolved from dependency injection container, and the value of property
                DefaultAppId in resolved object will be used in if condition-->
            <if parameter1="_classMember:IAppIds.DefaultAppId">
                <int32 value="14" />
            </if>

            <default>
                <int32 value="1"/>
            </default>
        </autoMethod>
    </autoService>

Example 3: Using **classMember** to call methods with parameters
================================================================

If the class member is a method, we can use **parameters** child element to specify parameter values when the method is called.

See the usage of **classMember** elements in the example below.

.. code-block:: xml
    :linenos:

    <autoService interface="IoC.Configuration.Tests.ClassMember.Services.IAppInfos">
      <autoProperty name="AllAppInfos"
          returnType="System.Collections.Generic.IReadOnlyList[....Services.IAppInfo]" >
        <collection>
          <!--
          An example of calling a non static factory method to create an instance of
          IAppInfo. Since method IAppInfoFactory.CreateAppInfo(appId, appDescription)
          is non-static, an instance of IAppInfoFactory will be resolved using the DI
          container.
          Also, since IAppInfoFactory is an interface, a binding for IAppInfoFactory
          should be configured in configuration file or in some module.
          -->
          <classMember class="...Tests.ClassMember.Services.IAppInfoFactory"
                       memberName="CreateAppInfo">
            <parameters>
              <int32 name="appId" value="1258"/>
              <string name="appDescription"
                      value="App info created with non-static method call."/>
            </parameters>
          </classMember>
          <!--
          An example of calling a static factory method to create an instance
          of IAppInfo.
          -->
          <classMember class="....Tests.ClassMember.Services.StaticAppInfoFactory"
                       memberName="CreateAppInfo">
            <parameters>
              <int32 name="appId" value="1259"/>
              <string name="appDescription"
                      value="App info created with static method call."/>
            </parameters>
          </classMember>
        </collection>
      </autoProperty>
    </autoService>