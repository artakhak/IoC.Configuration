============
Sample Files
============

- The XML configuration file schema is available at `IoC.Configuration.Schema.2F7CE7FF-CB22-40B0-9691-EAC689C03A36.xsd <https://github.com/artakhak/IoC.Configuration/blob/master/IoC.Configuration/IoC.Configuration.Content/IoC.Configuration.Schema.2F7CE7FF-CB22-40B0-9691-EAC689C03A36.xsd>`_.

- A template XML configuration file can be downloaded from `IoC.Configuration.Template.xml <https://github.com/artakhak/IoC.Configuration/blob/master/IoC.Configuration/IoC.Configuration.Content/IoC.Configuration.Template.xml>`_.
    .. note::
        The template file and schema are also available in folder, where **Nuget** package **IoC.Cnfiguration** is unpacked.

- The XML configuration files listed below are used in some examples throughout the documentation. The file **IoCConfiguration_Overview.xml** provides an overview of various features of **IoC.Configuration**, while the other configuration files (e.g., **IoCConfiguration_collection.xml**, **IoCConfiguration_constructedValue.xml**), provide additional examples for some features.
    .. note::
        These configuration files can be found in test project `IoC.Configuration.Tests <https://github.com/artakhak/IoC.Configuration/blob/master/IoC.Configuration.Tests>`_. Most of type names in configuration files are meaningless (e.g., Interface1, Class1, etc)


.. toctree::

   xml-configuration-file-schema.rst
   xml-configuration-template.rst
   IoCConfiguration_Overview.rst
   IoCConfiguration_GenericTypesAndTypeReUse.rst
   IoCConfiguration_autoService.rst
   IoCConfiguration_proxyService.rst
   IoCConfiguration_valueImplementation.rst
   IoCConfiguration_collection.rst
   IoCConfiguration_constructedValue.rst
   IoCConfiguration_classMember.rst
   IoCConfiguration_settingValue_ReferencingInConfiguration.rst