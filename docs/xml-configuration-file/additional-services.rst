=================================================
Additional Services Provided by IoC.Configuration
=================================================

In addition to registering types specified in XML configuration file and module classes, **IoC.Configuration** registers bindings for number of other interfaces.
Instances of these interfaces can be accessed using constructor or propertuy injection.

Here are some of the interfaces mentioned above:

- **IoC.Configuration.DiContainer.IDiContainer**: The **IoC** container.
- **IoC.Configuration.ConfigurationFile.IConfiguration**: Stores the structure of XML configuration file.
- **ISettings**: Stores the settings specified in element **iocConfiguration/settings** (see :doc:`./settings` for more details on settings).
- **IoC.Configurati.IPluginDataRepository** and **IoC.Configuration.ConfigurationFile.IPluginsSetup**: Provides access to registered plugins data (see :doc:`./plugins` for more details on plugins).