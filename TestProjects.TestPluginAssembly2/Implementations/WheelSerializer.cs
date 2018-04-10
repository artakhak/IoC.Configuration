using System;
using OROptimizer.Serializer;
using TestPluginAssembly2.Interfaces;

namespace TestPluginAssembly2.Implementations
{
    public class WheelSerializer : ITypeBasedSimpleSerializer
    {
        #region ITypeBasedSimpleSerializer Interface Implementation

        public Type SerializedType => typeof(IWheel);

        public bool TryDeserialize(string valueToDeserialize, out object deserializedValue)
        {
            deserializedValue = null;
            var items = valueToDeserialize.Split(',');

            if (items.Length != 2)
                return false;

            if (int.TryParse(items[0], out var color) && double.TryParse(items[1], out var height))
            {
                deserializedValue = new Wheel(color, height);
                return true;
            }

            return true;
        }

        public bool TrySerialize(object valueToSerialize, out string serializedValue)
        {
            serializedValue = string.Empty;

            var door = valueToSerialize as IWheel;
            if (door == null)
                return false;

            serializedValue = $"{door.Color},{door.Height}";
            return true;
        }

        #endregion
    }
}