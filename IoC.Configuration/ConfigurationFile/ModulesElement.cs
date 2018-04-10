using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class ModulesElement : ConfigurationFileElementAbstr, IModulesElement
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<Type, IModuleElement> _moduleTypeToModuleSetting = new Dictionary<Type, IModuleElement>();

        #endregion

        #region  Constructors

        public ModulesElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IModulesElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IModuleElement)
            {
                var moduleElement = (IModuleElement) child;

                if (moduleElement.Enabled && moduleElement.DiModule != null)
                {
                    var moduleType = moduleElement.DiModule.GetType();

                    if (_moduleTypeToModuleSetting.ContainsKey(moduleType))
                        throw new ConfigurationParseException(moduleElement, $"Multiple occurrences of dependency injection module '{moduleType.FullName}'.", this);

                    _moduleTypeToModuleSetting[moduleType] = moduleElement;
                }
            }
        }

        public IEnumerable<IModuleElement> Modules => _moduleTypeToModuleSetting.Values;

        #endregion
    }
}