// This software is part of the IoC.Configuration library
// Copyright � 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    public class ModuleElement : ConfigurationFileElementAbstr, IModuleElement
    {
        #region Member Variables

        [NotNull]
        private readonly ICreateInstanceFromTypeAndConstructorParameters _createInstanceFromTypeAndConstructorParameters;

        private bool _isDiManagerInactive;

        [CanBeNull]
        private IParameters _parameters;

        //private IAssemblyElement _assembly;

        [NotNull]
        private readonly ITypeHelper _typeHelper;

        private ITypeInfo _typeInfo;

        #endregion

        #region  Constructors

        public ModuleElement([NotNull] XmlElement xmlElement, [CanBeNull] IConfigurationFileElement parent,
                             [NotNull] ITypeHelper typeHelper,
                             [NotNull] ICreateInstanceFromTypeAndConstructorParameters createInstanceFromTypeAndConstructorParameters) : base(xmlElement, parent)
        {
            _typeHelper = typeHelper;
            _createInstanceFromTypeAndConstructorParameters = createInstanceFromTypeAndConstructorParameters;
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
                    if (parameter.IsResolvedFromDiContainer)
                        throw new ConfigurationParseException(parameter, $"Injected parameters cannot be used in element '{ElementName}'", this);
            }
        }

        public object DiModule { get; private set; }

        public override bool Enabled => base.Enabled && !_isDiManagerInactive;

        public override void Initialize()
        {
            base.Initialize();

            _typeInfo = _typeHelper.GetTypeInfo(this, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly, ConfigurationFileAttributeNames.TypeRef);

            if (_typeInfo.Assembly.Plugin != null)
            {
                if (OwningPluginElement == null)
                    throw new ConfigurationParseException(this, $"Assembly '{_typeInfo.Assembly.Name}' with alias '{_typeInfo.Assembly.Alias}' belongs to plugin '{_typeInfo.Assembly.Plugin.Name}'. The module should be declared under plugin element '{ConfigurationFileElementNames.PluginsSetup}/{ConfigurationFileElementNames.PluginSetup}' for plugin '{_typeInfo.Assembly.Plugin.Name}'.");
                
                if (_typeInfo.Assembly.Plugin != OwningPluginElement)
                    throw new ConfigurationParseException(this, $"Assembly '{_typeInfo.Assembly.Name}' with alias '{_typeInfo.Assembly.Alias}' is not in a folder dedicated for plugin '{OwningPluginElement.Name}'. To use this module in plugin '{OwningPluginElement.Name}', move the assembly to plugin folder: '{Path.Combine(OwningPluginElement.Configuration.Plugins.PluginsDirectory, OwningPluginElement.Name)}'");
            }
        }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            Type validBaseType = null;

            if (typeof(IDiModule).IsAssignableFrom(_typeInfo.Type))
            {
                validBaseType = typeof(IDiModule);
            }
            else if (_configuration.DiManagers.ActiveDiManagerElement.DiManager.ModuleType.IsAssignableFrom(_typeInfo.Type))
            {
                validBaseType = _configuration.DiManagers.ActiveDiManagerElement.DiManager.ModuleType;
            }
            else
            {
                _isDiManagerInactive = true;

                // If this is a native module, lets see if there is any non-active dependency manager module,
                // for which the type is correct.

                IDiManagerElement ownerDiManagerElement = null;

                var validModuleTypes = new List<Type>();
                foreach (var diManagerElement in _configuration.DiManagers.AllDiManagers)
                {
                    validModuleTypes.Add(diManagerElement.DiManager.ModuleType);

                    if (diManagerElement == _configuration.DiManagers.ActiveDiManagerElement)
                        continue;

                    if (diManagerElement.DiManager.ModuleType.IsAssignableFrom(_typeInfo.Type))
                    {
                        ownerDiManagerElement = diManagerElement;
                        validBaseType = diManagerElement.DiManager.ModuleType;
                        break;
                    }
                }

                if (ownerDiManagerElement == null)
                {
                    var errorMessage = new StringBuilder();
                    errorMessage.Append($"Invalid type for module: '{_typeInfo.TypeCSharpFullName}'.The type used as a module should be one of the following types or a subclass of any of these types: '{typeof(IDiModule).FullName}'");

                    foreach (var valdModuleType in validModuleTypes)
                        errorMessage.Append($", '{valdModuleType.FullName}'");

                    errorMessage.Append(".");

                    throw new ConfigurationParseException(this, errorMessage.ToString());
                }

                LogHelper.Context.Log.DebugFormat("Note, the module '{0}' is disabled since dependency injection manager '{1}' that handles the module is not the active dependency injection manager. The active dependency injection manager is '{2}'.",
                    _typeInfo.TypeCSharpFullName, ownerDiManagerElement.Name, _configuration.DiManagers.ActiveDiManagerElement.Name);
            }

            if (validBaseType != null && !_isDiManagerInactive && this.Enabled)
            {
                DiModule = _createInstanceFromTypeAndConstructorParameters.CreateInstance(this, validBaseType, _typeInfo.Type, _parameters?.AllParameters ?? new IParameterElement[0]);
                LogHelper.Context.Log.InfoFormat("Created an instance of dependency injection module: {0}.", _typeInfo.TypeCSharpFullName);
            }
        }

        #endregion
    }
}