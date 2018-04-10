using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class Configuration : ConfigurationFileElementAbstr, IConfiguration
    {
        #region  Constructors

        public Configuration([NotNull] XmlElement xmlElement) : base(xmlElement, null)
        {
        }

        #endregion

        #region IConfiguration Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IApplicationDataDirectory)
                ApplicationDataDirectory = (IApplicationDataDirectory) child;
            else if (child is IPlugins)
                Plugins = (IPlugins) child;
            else if (child is IAdditionalAssemblyProbingPaths)
                AdditionalAssemblyProbingPaths = (IAdditionalAssemblyProbingPaths) child;
            else if (child is IAssemblies)
                Assemblies = (IAssemblies) child;
            else if (child is IParameterSerializers)
                ParameterSerializers = (IParameterSerializers) child;
            else if (child is IDiManagersElement)
                DiManagers = (IDiManagersElement) child;
            else if (child is ISettingsElement)
                SettingsElement = (ISettingsElement) child;
            else if (child is IDependencyInjection)
                DependencyInjection = (IDependencyInjection) child;
            else if (child is ISettingsRequestorImplementationElement)
                SettingsRequestor = (ISettingsRequestorImplementationElement) child;
            else if (child is IStartupActionsElement)
                StartupActions = (IStartupActionsElement) child;
            else if (child is IPluginsSetup)
                PluginsSetup = (IPluginsSetup) child;
        }

        public IAdditionalAssemblyProbingPaths AdditionalAssemblyProbingPaths { get; private set; }
        public IApplicationDataDirectory ApplicationDataDirectory { get; private set; }
        public IAssemblies Assemblies { get; private set; }

        public IDependencyInjection DependencyInjection { get; private set; }
        public IDiManagersElement DiManagers { get; private set; }
        public IParameterSerializers ParameterSerializers { get; private set; }
        public IPlugins Plugins { get; private set; }
        public IPluginsSetup PluginsSetup { get; private set; }

        public void ProcessTree(ProcessConfigurationFileElement processConfigurationFileElement)
        {
            var stopProcessing = false;
            ProcessTree(this, processConfigurationFileElement, ref stopProcessing);
        }

        public ISettingsElement SettingsElement { get; private set; }
        public ISettingsRequestorImplementationElement SettingsRequestor { get; private set; }
        public IStartupActionsElement StartupActions { get; private set; }

        #endregion

        #region Member Functions

        private void ProcessTree(IConfigurationFileElement configurationFileElement, ProcessConfigurationFileElement processConfigurationFileElement, ref bool stopProcessing)
        {
            if (stopProcessing && !configurationFileElement.Enabled)
                return;

            processConfigurationFileElement(configurationFileElement, ref stopProcessing);

            if (stopProcessing)
                return;

            foreach (var childElement in configurationFileElement.Children)
            {
                if (!childElement.Enabled)
                    continue;

                ProcessTree(childElement, processConfigurationFileElement, ref stopProcessing);

                if (stopProcessing)
                    return;
            }
        }

        #endregion
    }
}