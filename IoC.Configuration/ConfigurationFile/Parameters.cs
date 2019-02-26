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

using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.ConfigurationFile
{
    public class Parameters : ConfigurationFileElementAbstr, IParameters
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<string, IParameterElement> _parameterNameToParameterMap = new Dictionary<string, IParameterElement>(StringComparer.Ordinal);

        [NotNull]
        [ItemNotNull]
        private readonly List<IParameterElement> _parameters = new List<IParameterElement>();

        #endregion

        #region  Constructors

        public Parameters([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IParameters Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameterElement parameterElement)
            {
                if (_parameterNameToParameterMap.ContainsKey(parameterElement.Name))
                    throw new ConfigurationParseException(parameterElement, $"Multiple occurrences of parameter with name '{parameterElement.Name}'.", this);

                _parameterNameToParameterMap[parameterElement.Name] = parameterElement;
                _parameters.Add(parameterElement);
            }
        }

        public IEnumerable<IParameterElement> AllParameters => _parameters;

        [Obsolete("Will be removed after 5/31/2019.")]
        public ParameterInfo[] GetParameterValues()
        {
            var parameterInfos = new ParameterInfo[_parameters.Count];

            for (var i = 0; i < _parameters.Count; ++i)
            {
                var parameter = _parameters[i];
                parameterInfos[i] = new ParameterInfo(parameter.ValueTypeInfo.Type, parameter.DeserializedValue);
            }

            return parameterInfos;
        }

        #endregion
    }
}