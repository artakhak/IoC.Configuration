using System;

namespace IoC.Configuration
{
    /// <summary>
    /// Represents the data structure used to provide information for resolving and initializing values dynamically
    /// within a configuration file structure in the IoC framework.
    /// </summary>
    public interface IProvidedValueData
    {
        /// <summary>
        /// Provided value type.
        /// </summary>
        Type Type { get; }
        
        /// <summary>
        /// Provided value name.<br/>
        /// - If the value of <see cref="ProvidedValueTargetType"/> is <see cref="IoC.Configuration.ProvidedValueTargetType.Setting"/>, this value is the setting name.<br/>
        /// - if the value of <see cref="ProvidedValueTargetType"/> is <see cref="IoC.Configuration.ProvidedValueTargetType.Property"/>, this value is the property name.<br/>
        /// - if the value of <see cref="ProvidedValueTargetType"/> is <see cref="IoC.Configuration.ProvidedValueTargetType.ConstructorParameter"/>, this value is the constructor parameter name.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Represents the type of target for which a provided value is intended.<br/>
        /// - <see cref="IoC.Configuration.ProvidedValueTargetType.Setting"/>: Indicates the target is a configuration setting.<br/>
        /// - <see cref="IoC.Configuration.ProvidedValueTargetType.Property"/>: Indicates the target is a property.<br/>
        /// - <see cref="IoC.Configuration.ProvidedValueTargetType.ConstructorParameter"/>: Indicates the target is a constructor parameter.
        /// </summary>
        ProvidedValueTargetType ProvidedValueTargetType { get; }
    }
}