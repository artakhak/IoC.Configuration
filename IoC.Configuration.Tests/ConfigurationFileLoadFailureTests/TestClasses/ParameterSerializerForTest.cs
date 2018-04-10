using System;
using OROptimizer.Serializer;

namespace IoC.Configuration.Tests.ConfigurationFileLoadFailureTests.TestClasses
{
    [Obsolete("If this class is not used, delete")]
    public class ParameterSerializerForTest : ITypeBasedSimpleSerializer
    {
        #region ITypeBasedSimpleSerializer Interface Implementation

        public Type SerializedType => typeof(ParameterSerializerForTest);

        public bool TryDeserialize(string valueToDeserialize, out object deserializedValue)
        {
            deserializedValue = null;
            if (!int.TryParse(valueToDeserialize, out var parsedInt))
                return false;

            deserializedValue = new SerializedTypeForTest(parsedInt);
            return true;
        }

        public bool TrySerialize(object valueToSerialize, out string serializedValue)
        {
            serializedValue = null;
            var serializedObject = valueToSerialize as SerializedTypeForTest;
            if (serializedObject == null)
                return false;

            serializedValue = $"{serializedObject.Property1}";
            return true;
        }

        #endregion
    }

    [Obsolete("If this class is not used, delete")]
    public class SerializedTypeForTest
    {
        #region  Constructors

        public SerializedTypeForTest(int param1)
        {
            Property1 = param1;
        }

        #endregion

        #region Member Functions

        public int Property1 { get; set; }

        #endregion
    }
}