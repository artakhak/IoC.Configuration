=================================
.NET Maui Application Integration
=================================

IoC.Configuration provides specialized support for .NET MAUI applications via the ``IoC.Configuration.Maui`` library.

Integration Process
-------------------

MAUI applications use a ``MauiAppBuilder`` to configure services. IoC.Configuration hooks into this builder using the ``IIntegratesWithHostBuilder`` interface. This ensures that when MAUI attempts to resolve Pages or ViewModels, it uses the IoC.Configuration container.

Example
-------

For a complete implementation, refer to the example project:
``K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\ApplicationIntegrationDemos\Maui\MauiDemo\MauiDemo.csproj``

Check the ``MauiProgram.cs`` file in the demo project to see how ``DiContainerBuilder`` is used to wrap the ``MauiAppBuilder``.