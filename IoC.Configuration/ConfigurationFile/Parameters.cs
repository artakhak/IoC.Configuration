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

            if (child is IParameterElement)
            {
                var parameter = (IParameterElement) child;

                if (parameter.Enabled)
                {
                    if (_parameterNameToParameterMap.ContainsKey(parameter.Name))
                        throw new ConfigurationParseException(parameter, $"Multiple occurrences of parameter with name '{parameter.Name}'.", this);

                    _parameterNameToParameterMap[parameter.Name] = parameter;
                    _parameters.Add(parameter);
                }
            }
        }

        public IEnumerable<IParameterElement> AllParameters => _parameters;

        public ParameterInfo[] GetParameterValues()
        {
            var parameterInfos = new ParameterInfo[_parameters.Count];

            for (var i = 0; i < _parameters.Count; ++i)
            {
                var parameter = _parameters[i];
                parameterInfos[i] = new ParameterInfo(parameter.ValueType, parameter.DeserializedValue);
            }

            return parameterInfos;
        }

        #endregion
    }
}