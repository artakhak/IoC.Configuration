using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    public class ModuleElement : ConfigurationFileElementAbstr, IModuleElement
    {
        #region Member Variables

        private IAssembly _assembly;

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        private bool _isDiManagerInactive;

        [CanBeNull]
        private IParameters _parameters;

        #endregion

        #region  Constructors

        public ModuleElement([NotNull] XmlElement xmlElement, [CanBeNull] IConfigurationFileElement parent,
                             [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region IModuleElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameters)
            {
                _parameters = (IParameters) child;

                foreach (var parameter in _parameters.AllParameters)
                    if (parameter.ValueInstantiationType == ValueInstantiationType.ResolveFromDiContext)
                        throw new ConfigurationParseException(parameter, $"Injected parameters cannot be used in element '{ElementName}'", this);
            }
        }

        public object DiModule { get; private set; }

        public override bool Enabled => base.Enabled && _assembly.Enabled && !_isDiManagerInactive;

        public override void Initialize()
        {
            base.Initialize();

            var assemblyAlias = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Assembly);

            _assembly = _configuration.Assemblies.GetAssemblyByAlias(assemblyAlias);

            if (_assembly == null)
                throw new ConfigurationParseException(this, $"No assembly with alias '{assemblyAlias}' is specified in element '{ConfigurationFileElementNames.Assemblies}'.");

            if (OwningPluginElement == null)
            {
                if (_assembly.Plugin != null)
                    throw new ConfigurationParseException(this, $"Assembly '{_assembly.Name}' with alias '{assemblyAlias}' belongs to plugin '{_assembly.Plugin.Name}'. The module should be declared under plugin element '{ConfigurationFileElementNames.PluginsSetup}/{ConfigurationFileElementNames.PluginSetup}' for plugin '{_assembly.Plugin.Name}'.");
            }
            else
            {
                if (_assembly.Plugin != OwningPluginElement)
                    throw new ConfigurationParseException(this, $"Assembly '{_assembly.Name}' with alias '{assemblyAlias}' is not in a folder dedicated for plugin '{OwningPluginElement.Name}'. To use this module in plugin '{OwningPluginElement.Name}', move the assembly to plugin folder: '{Path.Combine(OwningPluginElement.Configuration.Plugins.PluginsDirectory, OwningPluginElement.Name)}'");
            }
        }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            if (Enabled)
            {
                var moduleType = Helpers.GetTypeInAssembly(_assemblyLocator, this, _assembly,
                    this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Type));

                Type validBaseType = null;

                if (typeof(IDiModule).IsAssignableFrom(moduleType))
                {
                    validBaseType = typeof(IDiModule);
                }
                else if (_configuration.DiManagers.ActiveDiManagerElement.DiManager.ModuleType.IsAssignableFrom(moduleType))
                {
                    validBaseType = _configuration.DiManagers.ActiveDiManagerElement.DiManager.ModuleType;
                }
                else
                {
                    _isDiManagerInactive = true;

                    // If this is a native module, lets see if there is any non-active dependency manager module,
                    // for ehich the type is correct.

                    IDiManagerElement ownerDiManagerElement = null;

                    var validModuleTypes = new List<Type>();
                    foreach (var diManagerElement in _configuration.DiManagers.AllDiManagers)
                    {
                        if (!diManagerElement.Enabled)
                            continue;

                        validModuleTypes.Add(diManagerElement.DiManager.ModuleType);

                        if (diManagerElement == _configuration.DiManagers.ActiveDiManagerElement)
                            continue;

                        if (diManagerElement.DiManager.ModuleType.IsAssignableFrom(moduleType))
                        {
                            ownerDiManagerElement = diManagerElement;
                            validBaseType = diManagerElement.DiManager.ModuleType;
                            break;
                        }
                    }

                    if (ownerDiManagerElement == null)
                    {
                        var errorMessage = new StringBuilder();
                        errorMessage.Append($"Invalid type for module: '{moduleType.FullName}'.The type used as a module should be one of the following types or a subclass of any of these types: '{typeof(IDiModule).FullName}'");

                        foreach (var valdModuleType in validModuleTypes)
                            errorMessage.Append($", '{valdModuleType.FullName}'");

                        errorMessage.Append(".");

                        throw new ConfigurationParseException(this, errorMessage.ToString());
                    }

                    LogHelper.Context.Log.DebugFormat("Note, the module '{0}' is disabled since dependency injection manager '{1}' that handles the module is not the active dependency injection manager. The active dependency injection manager is '{2}'.",
                        moduleType.FullName, ownerDiManagerElement.Name, _configuration.DiManagers.ActiveDiManagerElement.Name);
                }

                if (validBaseType != null && !_isDiManagerInactive)
                {
                    if (!GlobalsCoreAmbientContext.Context.TryCreateInstanceFromType(validBaseType, moduleType, _parameters == null ? new ParameterInfo[0] : _parameters.GetParameterValues(), out var deserializedObject, out var errorMessage))
                        throw new ConfigurationParseException(this, errorMessage);

                    DiModule = deserializedObject;

                    LogHelper.Context.Log.InfoFormat("Created an instance of dependency injection module: {0}.",
                        moduleType.FullName);
                }
            }
        }

        #endregion
    }
}