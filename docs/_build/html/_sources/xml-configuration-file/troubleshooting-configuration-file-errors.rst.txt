=========================================
Troubleshooting Configuration File Errors
=========================================

**IoC.Configuration** logs helpfull tips on source of configuration file load errors.

    .. note::
        Refer to :doc:`../loading-ioc-configuration/index` for details on how to setup the logger.

Example of error logs is shown below:

.. code-block:: rst

    Fatal:
    Exception:Error in element 'settings':
    Required setting 'Int32Setting1' should be of type 'System.Int32'. Actual type is 'System.String'. The setting is in plugin settings for plugin 'Plugin1'. To fix the issue, either modify the implementation of method 'RequiredSettings' in class 'TestPluginAssembly1.Implementations.Plugin1', or add the setting.
    Element location in configuration file:
    <iocConfiguration>
    	<pluginsSetup>
    		<pluginSetup plugin="Plugin1">
    			<settings>
    				<int64 name="Int64Setting1" value="38"/>
    				<string name="StringSetting1" value="String Value 1"/>
    				<string name="Int32Setting1" value="some text"/> <--- Element 'string' is the 3-th child element of element 'settings'.

       at IoC.Configuration.DiContainerBuilder.FileBased.FileBasedConfiguration.ValidateRequiredSettings(ISettings settings, ISettingsRequestor settingsRequestor)
       at IoC.Configuration.DiContainerBuilder.FileBased.FileBasedConfiguration.ValidateRequiredSettings(IDiContainer diContainer)
       at IoC.Configuration.DiContainerBuilder.FileBased.FileBasedConfiguration.OnContainerStarted()
       at IoC.Configuration.DiContainerBuilder.DiContainerBuilderConfiguration.StartContainer()