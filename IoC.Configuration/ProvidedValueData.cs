using System;

namespace IoC.Configuration
{
    /// <inheritdoc />
    public class ProvidedValueData : IProvidedValueData
    {
        public ProvidedValueData(Type type, string name, ProvidedValueTargetType? providedValueTargetType)
        {
            Type = type;
            Name = name;
            ProvidedValueTargetType = providedValueTargetType;
        }

        /// <inheritdoc />
        public Type Type { get; }
        
        /// <inheritdoc />
        public string Name { get; }
        
        /// <inheritdoc />
        public ProvidedValueTargetType? ProvidedValueTargetType { get; }
    }
}