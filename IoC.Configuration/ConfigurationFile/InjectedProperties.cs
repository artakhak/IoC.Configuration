using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class InjectedProperties : ConfigurationFileElementAbstr, IInjectedProperties
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<string, IInjectedPropertyElement> _propertyNameToPropertyMap = new Dictionary<string, IInjectedPropertyElement>(StringComparer.Ordinal);

        #endregion

        #region  Constructors

        public InjectedProperties([NotNull] XmlElement xmlElement, IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IInjectedProperties Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IInjectedPropertyElement)
            {
                var property = (IInjectedPropertyElement) child;

                if (_propertyNameToPropertyMap.ContainsKey(property.Name))
                    throw new ConfigurationParseException(property, $"Multiple occurrences of property with name '{property.Name}'.", this);

                _propertyNameToPropertyMap[property.Name] = property;
            }
        }

        public IEnumerable<IInjectedPropertyElement> AllProperties => _propertyNameToPropertyMap.Values;

        #endregion
    }
}