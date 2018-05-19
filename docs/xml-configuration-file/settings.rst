========
Settings
========

.. contents::
  :local:
  :depth: 2

The configuration file has two elements for specifying settings: **/iocConfiguration/settingsRequestor** and **/iocConfiguration/settings**
    .. note::
        See :doc:`./plugins` for details on settings in plugins.

Here is an example of settings in XML configuration file:

.. code-block:: xml
    :linenos:

        <iocConfiguration>

            <settingsRequestor type="SharedServices.FakeSettingsRequestor"
                                assembly="shared_services">
            </settingsRequestor>

            <settings>
                <int32 name="SynchronizerFrequencyInMilliseconds" value="5000" />
                <double name="MaxCharge" value="155.7" />
                <string name="DisplayValue" value="Some display value" />
            </settings>
        </iocConfiguration>

- Element **settings** lists all the settings using elements **byte**, **int16**, **int32**, **int64**, **double**, **boolean**, **datetime**, **string**, **object**.
  The values of settings are de-serialized using serializers provided in element **parameterSerializers** (reference section :doc:`./parameter-serializers`).

- Element **settingsRequestor** is optional and if present, is used to force the user to include settings using the type specified in attributes **type** and **assembly**. The specified type should implement an interface **IoC.Configuration.ISettingsRequestor**, which specifies a collection of required settings that should be present in settings element.

     .. note::
            Note, the type specified in **type** attribute in **settingsRequestor** element is fully integrated into a dependency injection framework. In other words, constructor parameters will be injected using bindings specified in dependencyInjection element.


Interface IoC.Configuration.ISettingsRequestor
==============================================

.. code-block:: xml
    :linenos:

        namespace IoC.Configuration
        {
            public interface ISettingsRequestor
            {
                /// <summary>
                /// Gets the collection of settings, that should be
                /// present in configuration file.
                /// </summary>
                [NotNull, ItemNotNull]
                IEnumerable<SettingInfo> RequiredSettings { get; }
            }
        }

Accessing Setting Values in Code
================================

To access the setting values in code, inject the type **IoC.Configuration.ISettings** as a constructor parameter, and use the methods **bool GetSettingValue<T>(string name, T defaultValue, out T value)** or **T GetSettingValueOrThrow<T>(string name)** in **IoC.Configuration.ISettings**.

Here is an example:

.. code-block:: csharp
    :linenos:

        public class TestInjectedSettings
        {
            public TestInjectedSettings(ISettings settings)
            {
                Assert.IsTrue(settings.GetSettingValue<double>("MaxCharge", 5.3,
                                                out var maxChargeSettingValue));
                Assert.AreEqual(155.7, maxChargeSettingValue);

                Assert.IsFalse(settings.GetSettingValue<int>("MaxCharge", 5,
                                                out var settingValueNotFound_InvalidType));
                Assert.AreEqual(5, settingValueNotFound_InvalidType);

                Assert.IsFalse(settings.GetSettingValue<int>("MaxChargeInvalid", 7,
                                                out var nonExistentSettingValue));
                Assert.AreEqual(7, nonExistentSettingValue);

                try
                {
                    // This call will throw an exception, since there is no setting of double
                    // type with name "MaxChargeInvalid".
                    settings.GetSettingValueOrThrow<double>("MaxChargeInvalid");
                    Assert.Fail("An exception should have been thrown.");
                }
                catch
                {
                }
            }
        }

.. note::

    Binding for a type **TestInjectedSettings** should be registered either in module class or in XML configuration file. Below is an example of registering binding for **TestInjectedSettings** in an **IoC.Configuration** module.

        .. code-block:: csharp

            public class TestDiModule : IoC.Configuration.DiContainer.ModuleAbstr
            {
                protected override void AddServiceRegistrations()
                {
                    this.Bind<TestInjectedSettings>().ToSelf();
                }
            }
