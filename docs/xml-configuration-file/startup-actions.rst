===============
Startup Actions
===============

- The XML Configuration file has **iocConfiguration/startupActions/startupAction** elements for specifying any number of startup actions. Each **startupAction** element specifies a type that is an implementation of interface **IoC.Configuration.OnApplicationStart.IStartupAction**.
- When the XML configuration file is loaded **IoC.Configuration** will call the method **IoC.Configuration.OnApplicationStart.IStartupAction.Start()** for each startup action specified in **startupAction** elements.
- When the XML configuration file is disposed of (when **IoC.Configuration.DiContainerBuilder.IContainerInfo** is disposed of), **IoC.Configuration.OnApplicationStart.IStartupAction.Stop()** is called on each startup action.
- Startup actions are integrated into dependency injection mechanism. Therefore, the constructor parameters of **IoC.Configuration.OnApplicationStart.IStartupAction** implementations specified in **startupAction** elements will be injected using the bindings specified in XML Configuration file or in modules referenced by the configuration file. Also, **parameters** and **injectedProperties** elements can used with **startupActions** to specify constructor parameters or property injection.
- **IoC.Configuration** waits for up to 15 seconds, to make sure that all startup actions are given enough time to properly stop (e.g., stop the threads if necessary).
    .. note::
        If all startup actions have **false** value of property **IStartupAction.ActionExecutionCompleted** before 15 seconds passes, the wait time will be shorter.


Here is an example of startu action elements in configuration file:

.. code-block:: xml

    <startupActions>
        <startupAction type="DynamicallyLoadedAssembly1.Implementations.StartupAction1"
                       assembly="dynamic1">
          <!--Use parameters element to specify constructor parameters if necessary.-->
          <!--<parameters></parameters>-->
          <!--Use injectedProperties element to inject properties into startup action if necessary.-->
          <!--<injectedProperties></injectedProperties>-->
        </startupAction>
        <startupAction type="DynamicallyLoadedAssembly1.Implementations.StartupAction2"
                       assembly="dynamic1"></startupAction>
    </startupActions>

Here is the definition of interface **IoC.Configuration.OnApplicationStart.IStartupAction**

.. code-block:: csharp

    public interface IStartupAction
    {
        /// <summary>
        /// If <c>true</c>, the action was successfully stopped.
        /// </summary>
        bool ActionExecutionCompleted { get; }

        /// <summary>
        /// Starts the action.
        /// </summary>
        void Start();

        /// <summary>
        ///  Stops the action.
        /// </summary>
        void Stop();
    }