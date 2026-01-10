=======================
Application Integration
=======================

IoC.Configuration now supports seamless integration with various .NET application types by allowing the application's native dependency injection container to use IoC.Configuration for resolving services.

This integration is achieved through the use of the ``IIntegratesWithHostBuilder`` interface and specialized extension libraries for different frameworks. By hooking into the application's host building process, IoC.Configuration can manage service registrations and resolution transparently.

Overview
--------

The integration typically involves:

1. Initializing the `IoC.Configuration.DiContainerBuilder.DiContainerBuilder <https://github.com/artakhak/IoC.Configuration/blob/master/IoC.Configuration/DiContainerBuilder/DiContainerBuilder.cs>`_.
2. Configuring the file-based dependency injection.
3. Using the ``WithHostBuilder`` method to provide an implementation of `IoC.Configuration.DiContainerBuilder.IApplicationHostBuilder <https://github.com/artakhak/IoC.Configuration/blob/master/IoC.Configuration/DiContainerBuilder/IApplicationHostBuilder.cs>`_.
    
    .. note::
        The default implementation `IoC.Configuration.DiContainerBuilder.ApplicationHostBuilder <https://github.com/artakhak/IoC.Configuration/blob/master/IoC.Configuration/DiContainerBuilder/ApplicationHostBuilder.cs>`_ works in some frameworks, such as in WinUI3 applications, while some other frameworks such as ASP.NET and Maui require custom implementations provided in nuget packages `IoC.Configuration.AspNet <https://www.nuget.org/packages/IoC.Configuration.AspNet>`_ and `IoC.Configuration.Maui <https://www.nuget.org/packages/IoC.Configuration.Maui>`_.
        The source code of these packages can be found at these links: `IoC.Configuration.AspNet <https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration.AspNet>`_, `IoC.Configuration.Maui <https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration.Maui>`_.
4. Registering modules and starting the container.

Currently, this feature is primarily supported for file-based configurations.

Supported Frameworks
--------------------

* **ASP.NET Core**: Integration with the web host to resolve controllers and other services via IoC.Configuration. Refer to the example in project ``WebApiDemo.csproj`` and related projects in `ApplicationIntegrationDemos/ASP.NET.WebApi <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/ASP.NET.WebApi>`_.
* **.NET MAUI**: Integration with the Maui app builder. Refer to the example in project ``MauiDemo.csproj`` and  related projects in `ApplicationIntegrationDemos/Maui <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/Maui>`_.
* **WinUI 3**: Integration with WinUI 3 application hosting. Refer to the example in project ``WinUI3Demo.csproj`` and  related projects in `ApplicationIntegrationDemos/WinUI3 <https://github.com/artakhak/IoC.Configuration/blob/master/ApplicationIntegrationDemos/WinUI3>`_.

.. toctree::
    :maxdepth: 2

    asp-net/index.generated.rst
    maui/index.generated.rst
    winui3/index.generated.rst