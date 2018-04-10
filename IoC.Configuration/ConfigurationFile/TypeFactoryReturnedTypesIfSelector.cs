using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class TypeFactoryReturnedTypesIfSelector : TypeFactoryReturnedTypesSelector, ITypeFactoryReturnedTypesIfSelector
    {
        #region Member Variables

        [NotNull]
        [ItemNotNull]
        private readonly IList<string> _parameterValues = new List<string>(10);

        #endregion

        #region  Constructors

        public TypeFactoryReturnedTypesIfSelector([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region ITypeFactoryReturnedTypesIfSelector Interface Implementation

        public override void Initialize()
        {
            base.Initialize();

            var parameterIndexToValueMap = new SortedDictionary<int, XmlAttribute>();

            var attributeNamePrefix = "parameter";

            foreach (var attribute in _xmlElement.Attributes)
            {
                var xmlAttribute = attribute as XmlAttribute;

                if (xmlAttribute == null)
                    continue;

                if (xmlAttribute.Name.StartsWith(attributeNamePrefix) &&
                    int.TryParse(xmlAttribute.Name.Substring(attributeNamePrefix.Length), out var parameterIndex))
                    parameterIndexToValueMap[parameterIndex] = xmlAttribute;
            }

            var prevParameterIndex = 0;
            foreach (var keyValuePair in parameterIndexToValueMap)
            {
                if (prevParameterIndex + 1 != keyValuePair.Key)
                    throw new ConfigurationParseException(this, $"If parameter '{keyValuePair.Value.Name}' is specified, all the preceding parameters should be specified as well.");

                _parameterValues.Add(keyValuePair.Value.Value);
                prevParameterIndex = keyValuePair.Key;
            }
        }

        public IEnumerable<string> ParameterValues => _parameterValues;

        #endregion
    }
}