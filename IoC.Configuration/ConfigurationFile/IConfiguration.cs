using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IConfiguration : IConfigurationFileElement
    {
        #region Current Type Interface

        [CanBeNull]
        IAdditionalAssemblyProbingPaths AdditionalAssemblyProbingPaths { get; }

        [NotNull]
        IApplicationDataDirectory ApplicationDataDirectory { get; }

        [NotNull]
        IAssemblies Assemblies { get; }

        [CanBeNull]
        IDependencyInjection DependencyInjection { get; }

        [NotNull]
        IDiManagersElement DiManagers { get; }

        [NotNull]
        IParameterSerializers ParameterSerializers { get; }

        [CanBeNull]
        IPlugins Plugins { get; }

        [CanBeNull]
        IPluginsSetup PluginsSetup { get; }

        void ProcessTree(ProcessConfigurationFileElement processConfigurationFileElement);

        [CanBeNull]
        ISettingsElement SettingsElement { get; }

        [CanBeNull]
        ISettingsRequestorImplementationElement SettingsRequestor { get; }

        [CanBeNull]
        IStartupActionsElement StartupActions { get; }

        #endregion
    }
}