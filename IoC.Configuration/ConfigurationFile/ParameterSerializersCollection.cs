using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class ParameterSerializersCollection : ConfigurationFileElementAbstr, IParameterSerializersCollection
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<Type, IParameterSerializer> _typeHandledBySerializerToSerializer = new Dictionary<Type, IParameterSerializer>();

        #endregion

        #region  Constructors

        public ParameterSerializersCollection([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IParameterSerializersCollection Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameterSerializer)
            {
                var parameterSerializer = (IParameterSerializer) child;

                if (parameterSerializer.Enabled)
                {
                    if (_typeHandledBySerializerToSerializer.TryGetValue(parameterSerializer.Serializer.SerializedType, out var serializer))
                        throw new ConfigurationParseException(parameterSerializer,
                            $"Invalid serializer '{parameterSerializer.Serializer.GetType()}'. Configuration file has another serializer '{serializer.GetType().FullName}' for the same type '{parameterSerializer.Serializer.SerializedType.FullName}'.", this);

                    _typeHandledBySerializerToSerializer[parameterSerializer.Serializer.SerializedType] = parameterSerializer;
                }
            }
        }

        public IEnumerable<IParameterSerializer> AllSerializers => _typeHandledBySerializerToSerializer.Values;

        #endregion
    }
}