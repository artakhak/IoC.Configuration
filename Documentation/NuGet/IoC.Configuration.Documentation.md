Note, the source code of this package can be found at [https://github.com/artakhak/IoC.Configuration](https://github.com/artakhak/IoC.Configuration).
There is a test project **IoC.Configuration.Tests** at this location, demonstrating how to do file and code based configuration of dependency injection (the examples use IoC.Configuration.Autofac.Ninject extensions available in NuGet).

Also, GitHub project has some basic documentation. More documentation will be added soon.

The XML configuration file is validated against schema file **IoC.Configuration.Schema.2F7CE7FF-CB22-40B0-9691-EAC689C03A36.xsd**, which can be found in folder
**IoC.Cnfiguration.Content** in package directory or at [https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration/IoC.Cnfiguration.Content](https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration/IoC.Cnfiguration.Content).

A template XML configuration file can also be downloaded from [https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration/IoC.Cnfiguration.Content](https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration/IoC.Cnfiguration.Content).

Example of service binding in **IoC.Configuration** module:

***

Bind<IInterface5>().OnlyIfNotRegistered().To<Interface5_Impl1>().SetResolutionScope(DiResolutionScope.ScopeLifetime);

***

Example of service binding in XML configuration file:

***

...
<service type='SharedServices.Interfaces.ICleanupJobData' assembly='shared_services'>
    <implementation type='DynamicallyLoadedAssembly1.Implementations.CleanupJobData' 
					assembly='dynamic1' scope='singleton'>
    </implementation>
</service>
...
***

Also, both file and code based configurations allow specifying native modules (e.g., instances of Autofac.AutofacModule and  Ninject.Modules.NinjectModule).