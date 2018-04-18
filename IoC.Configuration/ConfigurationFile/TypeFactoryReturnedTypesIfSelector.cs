// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
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