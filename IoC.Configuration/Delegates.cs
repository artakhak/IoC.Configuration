using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainer.BindingsForCode;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public delegate void ProcessConfigurationFileElement(IConfigurationFileElement configurationFileElement, ref bool stopProcessing);

    public delegate void LifeTimeScopeTerminatedEventHandler([CanBeNull] object sender, [NotNull] LifeTimeScopeTerminatedEventArgs e);

    public delegate void BindingConfigurationAddedEventHandler([CanBeNull] object sender, [NotNull] BindingConfigurationAddedEventArgs e);

    public delegate void BindingImplementationConfigurationAddedEventHandler<TBindingImplementationConfiguration>([CanBeNull] object sender,
                                                                                                                  [NotNull] BindingImplementationConfigurationAddedEventArgs<TBindingImplementationConfiguration> e) where TBindingImplementationConfiguration : BindingImplementationConfiguration;

    public delegate void ConfigurationFileXmlDocumentLoadedEventHandler([CanBeNull] object sender, [NotNull] ConfigurationFileXmlDocumentLoadedEventArgs e);
}