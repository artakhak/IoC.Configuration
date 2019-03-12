==========================
**injectedObject** element
==========================

Element **injectedObject** can be used to specify a value that will be injected by **IoC.Configuration**.

The type to inject is specified using either **type** attribute (and optional **assembly** attribute), or **typeRef** attribute.

If the type to inject is non-abstract and non-interface, and has a public constructor, **IoC.Configuration** will create a binding for the type.

Otherwise, a binding for the type should be specified either in configuration file, or in one of the loaded **IoC** modules.

Example 1: Using **injectedObject** element to specify service implementation constructor parameter value
=========================================================================================================

.. code-block:: xml
    :linenos:

    <service type="SharedServices.Interfaces.IInterface2" >
        <implementation type="SharedServices.Implementations.Interface2_Impl1"
                        scope="singleton">
            <parameters>
                <datetime name="param1" value="2014-10-29 23:59:59.099" />
                <double name="param2" value="125.1" />
                <injectedObject name="param3"
                                type="SharedServices.Interfaces.IInterface3" />
            </parameters>
        </implementation>
    </service>

Example 2: Using **injectedObject** element to specify injected property value in service implementation
========================================================================================================

.. code-block:: xml
    :linenos:

    <service type="SharedServices.Interfaces.IInterface2" >
        <implementation type="SharedServices.Implementations.Interface2_Impl2"
                        scope="singleton">
            <injectedProperties>
              <datetime name="Property1"
                        value="1915-04-24 00:00:00.001" />
              <double name="Property2" value="365.41" />
              <injectedObject name="Property3"
                              type="SharedServices.Interfaces.IInterface3" />
            </injectedProperties>
        </implementation>
    </service>

Example 2: Using **injectedObject** element to specify a returned value in **autoProperty** element
===================================================================================================

.. code-block:: xml
    :linenos:

    <autoService interface="IoC.Configuration.Tests.AutoService.Services.IActionValidatorFactory">
        <autoProperty name="DefaultActionValidator"
                      returnType="SharedServices.Interfaces.IActionValidator">
          <injectedObject
                type="IoC.Configuration.Tests.AutoService.Services.ActionValidatorDefault"/>
        </autoProperty>
    </autoService>