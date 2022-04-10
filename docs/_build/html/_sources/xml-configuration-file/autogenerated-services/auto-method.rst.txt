======================
Element **autoMethod**
======================

.. contents::
  :local:
  :depth: 2

Element **autoMethod** is used to configure the auto-generated implementation of a method in interface specified in element **autoService**.

An example of **autoMethod** elements is provided in :doc:`index`. In this section some specifics of this element will be provided.

The format of **autoMethod**
============================

Method name and return type in **autoMethod**
---------------------------------------------

- Method name is specified using the attribute **name**.
- Method return value type is specified using the attribute **returnType** and optional attribute **assembly**, or alternatively using attribute **returnTypeRef** to reference a type defined in sole **/iocConfiguration/typeDefinitions/typeDefinition** element.
    .. note::
        Even though we only need the method name and signature, to identify the method, the return type makes the configuration more readable. Also, the return type serves as an additional way to identify the method, if a method with similar name and signature exists in multiple extended interfaces.

The example below demonstrates how method name and return type are specified:

.. code-block:: xml
    :linenos:

    <autoMethod name="GetValidators"
                returnType="System.Collections.Generic.IReadOnlyList[SharedServices.Interfaces.IActionValidator]">

        <!--method signature and return value elements go here...-->
    </autoMethod>

Method signature
----------------

Child element **methodSignature** is used to specify the auto-implemented method signature.

If the method has no parameters, no **methodSignature** element is necessary. Otherwise, this element should be present, which should list the method parameters.

The example below demonstrates how method signature is specified:

.. code-block:: xml
    :linenos:

    <!--autoMethod below is a configuration for a C# method
    System.Collections.Generic.IReadOnlyList[SharedServices.Interfaces.IActionValidator] GetValidators(
                            SharedServices.DataContracts.ActionTypes actionType,
                            System.Guid projectGuid,
                            int someIntParam)-->
    <autoMethod name="GetValidators"
                returnType="System.Collections.Generic.IReadOnlyList[SharedServices.Interfaces.IActionValidator]">

        <methodSignature>
            <!--paramName attribute is optional, however it
                makes the auto-implementation more readable. -->
            <object paramName="actionType" typeRef="ActionTypes"/>
            <object paramName="projectGuid" type="System.Guid"/>
            <int32 paramName="someIntParam" />
        </methodSignature>

        <!--return value elements go here..-->
    </autoMethod>


Specifying the return values
----------------------------

Return values are specified by using any number of optional **if** elements, followed by required **default** element.

- Specifying a return value using element **if**

    Element **if** is used to specify a return value by providing values for up to 10 parameters using attributes **parameter1**, **parameter2**, ..., **parameter10**.

    Attribute **parameter1**  corresponds to the first parameter in **methodSignature** element, attribute **parameter2** corresponds to the second parameter in **methodSignature** element, and so on.

    The number of parameter attributes should be the same as the number of parameters in element **methodSignature**.

    The method auto-implemented by **IoC.Configuration** will return the value specified in child element of **if** element, if parameters in method call are equal to the values in attributes **parameter1**, **parameter2**, etc.
        .. note::
            The child of **if** element should be a value initializer element, such as **collection**, **int32**, **constructedValue**, **injectedObject**, etc. Refer to :doc:`../value-initialization-elements/index` for more details on value initializer elements.

    The value of parameter attribute is one of the following:

        - A value that will be de-serialized by a parameter serializer to a value of the parameter (example <if parameter1="15.3">).

            .. note::
                Refer to :doc:`../parameter-serializers` for more details on parameter serializers.

            Example (see **parameter2** with value "**8663708F-C707-47E1-AEDC-2CD9291AD4CB**"):

            .. code-block:: xml

                <!--The generated code will return a collection with two items of types
                    SharedServices.Implementations.ActionValidator1 and SharedServices.Implementations.ActionValidator3
                    if the first parameter value is SharedServices.DataContracts.ActionTypes.ViewFilesList, and the second parameter
                    value is a Guid "8663708F-C707-47E1-AEDC-2CD9291AD4CB" (note, this value will be serialized to System.Guid,
                    if the parameter is of type System.Guid).
                -->
                <if parameter1="_classMember:SharedServices.DataContracts.ActionTypes.ViewFilesList"
                    parameter2="8663708F-C707-47E1-AEDC-2CD9291AD4CB">
                    <collection>
                        <injectedObject type="SharedServices.Implementations.ActionValidator1" />
                        <injectedObject type="SharedServices.Implementations.ActionValidator3" />
                    </collection>
                </if>

        - A class member specified by using prefix **_classMember** followed by class member path. Class member path is the full name of the type (or the type alias for some type defined in **iocConfiguration/typeDefinitions/typeDefinition** element), followed by class member name.

            .. note::
                Refer to :doc:`../value-initialization-elements/class-member` for more details on how class members are resolved.

            .. note::
                Class member can be can be a member in the auto-generated service as well.

            Example (see **parameter1** with value "**_classMember:ActionTypes.ViewFileContents**"):

            .. code-block:: xml

                <!--The generated code will return a collection with one items of type
                    SharedServices.Implementations.ActionValidator1, if the first parameter
                    value is SharedServices.DataContracts.ActionTypes.ViewFileContents, and the second parameter
                    value is a Guid "F981F171-B382-4F15-A8F9-FE3732918D3F" (note, this value will be serialized to
                    System.Guid, if the parameter is of type System.Guid).
                -->
                <if parameter1="_classMember:ActionTypes.ViewFileContents"
                    parameter2="F981F171-B382-4F15-A8F9-FE3732918D3F">
                    <collection>
                        <injectedObject type="SharedServices.Implementations.ActionValidator1" />
                    </collection>
                </if>

        - A setting value specified by using prefix **_settings** followed by a setting name. A general, or plugin setting with specified name should exist in configuration file.

            .. note::
                Refer to :doc:`../settings` or :doc:`../plugins` for more details on general and plugin settings.

            Example (see **parameter2** with value "_settings:Project1Guid"):

            .. code-block:: xml

                <!--The generated code will return a collection with one item of type
                    SharedServices.Implementations.ActionValidator3, if the first parameter
                    value is SharedServices.DataContracts.ActionTypes.ViewFileContents, and the second parameter
                    is equal to the value of setting named **Project1Guid**.
                -->
                <if parameter1="_classMember:ActionTypes.ViewFileContents"
                    parameter2="_settings:Project1Guid">
                    <collection>
                        <injectedObject type="SharedServices.Implementations.ActionValidator3" />
                    </collection>
                </if>


- Specifying a return value using element **default**

    Element **default** is is used to specify a value to return, if none of the conditions specified in **if** elements is **true**, or if no **if** element is present.

    **IoC.Configuration** will return the value specified in child of **default** element.
        .. note::
            The child of **default** element should be a value initializer element, such as **collection**, **int32**, **constructedValue**, **injectedObject**, etc. Refer to :doc:`../value-initialization-elements/index` for more details on value initializer elements.

    Example:

    .. code-block:: xml

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
            <!--Some other autoMethod and autoproperty elements go here-->
        </autoService>

Referencing the auto-implemented method parameters
--------------------------------------------------

Element **parameterValue** is used to reference a parameter value in auto-implemented method of auto-generated service.

This element can be used only under elements **if** or **default** under element **autoMethod**.

The element uses an attribute **paramName** to reference the parameter of auto-generated method.
A parameter with this name should be declared under element **../autoService/autoMethod/methodSignature**.

Example:

.. code-block:: xml
    :linenos:

    <autoGeneratedServices>
      <!--Demo of referencing auto-implemented method parameters using
          parameterValue element-->
      <autoService
        interface="IoC.Configuration.Tests.AutoService.Services.IAppInfoFactory">

        <autoMethod name="CreateAppInfo"
                    returnType="IoC.Configuration.Tests.AutoService.Services.IAppInfo">

          <methodSignature>
            <int32 paramName="appId"/>
            <string paramName="appDescription"/>
          </methodSignature>

          <default>
            <constructedValue
                type="IoC.Configuration.Tests.AutoService.Services.AppInfo">
              <parameters>
                <!--The value of name attribute is the name of constructor parameter
                    in AppInfo-->
                <!--
                The value of paramName attribute is the name of parameter in
                IAppInfoFactory.CreateAppInfo.
                This parameter should be present under autoMethod/methodSignature element.
                -->
                <!--In this example the values of name and paramName are similar, however
                    they don't have to be.-->
                <parameterValue name="appId" paramName="appId" />
                <parameterValue name="appDescription" paramName="appDescription" />
              </parameters>
            </constructedValue>
          </default>
        </autoMethod>
      </autoService>
    </autoGeneratedServices>


Caching the returned values
---------------------------

If constructing the object returned by the function is time consuming, an optional attribute **reuseValue** in element **autoMethod** can be used to cache the returned values.

Example:

.. code-block:: xml
    :linenos:

    <autoService
        interface="TestPluginAssembly1.Interfaces.IResourceAccessValidatorFactory">
        <autoMethod name="GetValidators"
                    returnTypeRef="IEnumerableOfIResourceAccessValidator"
                    reuseValue="true" >
          <!--method signature and return value elements go here...-->
        </autoMethod>
        <!--Some other autoMethod and autoProperty elements go here -->
    </autoService>

Resolving conflicts by using **declaringInterface** in **autoMethod**
---------------------------------------------------------------------

If the auto-implemented method with the specified name, signature, and return type is not in auto-implemented interface, but is present in multiple parent interfaces, **IoC.Configuration** will report an error, since it will not know which method the configuration refers to.

In such rare cases an attribute **declaringInterface** can be used to specify explicitly the parent interface, where the method is declared.

Example:

    .. code-block:: xml
        :linenos:

        <!--
        IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo demonstrates cases
        when there are multiple occurrences of auto-generated methods and properties with
        same signatures and return types in its base interfaces.
        -->
        <autoService interface="IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo">

            <!--GetIntValues(): IReadOnlyList<int> GetIntValues(int param1, string param2)-->
            <autoMethod name="GetIntValues"
                        returnType="System.Collections.Generic.IReadOnlyList[System.Int32]" >
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
            This method is declared in IMemberAmbiguityDemo_Parent3, which is a base interface for
            IMemberAmbiguityDemo.
            We can provide implementation for this interface, even though it has a similar signature
            and return type as the method
            IoC.Configuration.Tests.AutoService.Services.IMemberAmbiguityDemo.GetIntValues.
            By using the attribute 'declaringInterface', we make a distinction between these two.
            -->
            <autoMethod name="GetIntValues"
                        returnType="System.Collections.Generic.IReadOnlyList[System.Int32]"
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
        </autoService>