==========================================
Predefined Type Value Initializer Elements
==========================================

Predefined type value elements are elements used to provide values for some predefined types, such as **System.Int32**, **System.Double**, **System.DateTime**, etc.

These elements have a name, that reflects the type of the value being specified (e.g., **<int32>**, <datetime>, etc).

The value specified in **value** attribute in these elements is serialized/deserialized using one of the classes in package **OROptimizer.Shared**, that implement **OROptimizer.Serializer.ITypeBasedSimpleSerializer**.

.. note::
    Refer to :doc:`../parameter-serializers` for more details on parameter serializers.

For example the value specified in element **datetime** is serialized using serialized/deserialized using the class **OROptimizer.Serializer.TypeBasedSimpleSerializerDateTime**.

The serializers used to serialize/deserialize the value in **value** element can be replaced, by specifying a different serializer in element **iocConfiguration/parameterSerializers/serializers/parameterSerializer** (see :doc:`../parameter-serializers`).

.. note::
    To see the serializers loaded by **IoC.Configuration** for different types, run the **IoC.Configuration** with logging level set to **INFO**.

The following is the overview of predefined value initialization elements:

- **byte**: Used to specify values of type **System.Byte**.
    The serializer class is **OROptimizer.Serializer.TypeBasedSimpleSerializerByte**.

- **int16**: Used to specify values of type **System.Int16**.
    The serializer class is **OROptimizer.Serializer.TypeBasedSimpleSerializerShort**.

- **int32**: Used to specify values of type **System.Int32**.
    The serializer class is **OROptimizer.Serializer.TypeBasedSimpleSerializerInt**.

- **int64**: Used to specify values of type **System.Int64**.
    The serializer class is **OROptimizer.Serializer.TypeBasedSimpleSerializerLong**.

- **double**: Used to specify values of type **System.Double**.
    The serializer class is **OROptimizer.Serializer.TypeBasedSimpleSerializerDouble**.

- **datetime**: Used to specify values of type **System.DateTime**.
    The serializer class is **OROptimizer.Serializer.TypeBasedSimpleSerializerLong**.

- **boolean**: Used to specify values of type **System.Boolean**.
    The serializer class is **OROptimizer.Serializer.TypeBasedSimpleSerializerBoolean**.

- **string**: Used to specify values of type **System.String**.
    The serializer class is **OROptimizer.Serializer.TypeBasedSimpleSerializerString**.

- **datetime**: Used to specify values of type **System.DateTime**.
        The serializer class is **OROptimizer.Serializer.TypeBasedSimpleSerializerLong**.


Example 1: Using **double** and **datetime** elements as service implementation constructor parameters
======================================================================================================

.. code-block:: xml
    :linenos:

    <service type="SharedServices.Interfaces.IInterface2" >
        <!--Test constructor parameters-->
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

Example 2: Using **int32** element to specify a return value in autoProperty element
====================================================================================

.. code-block:: xml
    :linenos:

    <autoService interfaceRef="IProjectIds" >
        <autoProperty name="DefaultProjectId" returnType="System.Int32">
            <int32 value="1"/>
        </autoProperty>
    </autoService>


Example 2: **int32**, **double**, and **string** elements in **settings** element
=================================================================================

.. code-block:: xml
    :linenos:

    <settings>
        <int32 name="SynchronizerFrequencyInMilliseconds" value="5000" />
        <double name="MaxCharge" value="155.7" />
        <string name="DisplayValue" value="Some display value" />
    </settings>

