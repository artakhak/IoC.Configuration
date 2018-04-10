using System;
using OROptimizer.Serializer;

namespace TestPluginAssembly1.Implementations
{
    public class UnsignedIntSerializerWithParameters : ITypeBasedSimpleSerializer
    {
        #region  Constructors

        public UnsignedIntSerializerWithParameters(int param1, double param2)
        {
            Property1 = param1;
            Property2 = param2;
        }

        #endregion

        #region ITypeBasedSimpleSerializer Interface Implementation

        public Type SerializedType => typeof(uint);

        public bool TryDeserialize(string valueToDeserialize, out object deserializedValue)
        {
            if (uint.TryParse(valueToDeserialize, out var deserializedValueLocal))
            {
                deserializedValue = deserializedValueLocal;
                return true;
            }

            deserializedValue = 0;
            return false;
        }

        public bool TrySerialize(object valueToSerialize, out string serializedValue)
        {
            if (valueToSerialize is uint)
            {
                serializedValue = valueToSerialize.ToString();
                return true;
            }

            serializedValue = null;
            return false;
        }

        #endregion

        #region Member Functions

        public int Property1 { get; }
        public double Property2 { get; }

        #endregion
    }
}