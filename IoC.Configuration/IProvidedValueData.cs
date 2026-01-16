using System;
using JetBrains.Annotations;

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
        /// If the value is not null, specifies the name of parameter, property, setting, etc. set by provided value.<br/>
        /// - If the value of <see cref="ProvidedValueTargetType"/> is <see cref="IoC.Configuration.ProvidedValueTargetType.Setting"/>, this value is the setting name.<br/>
        /// - if the value of <see cref="ProvidedValueTargetType"/> is <see cref="IoC.Configuration.ProvidedValueTargetType.Property"/>, this value is the property name.<br/>
        /// - if the value of <see cref="ProvidedValueTargetType"/> is <see cref="IoC.Configuration.ProvidedValueTargetType.ConstructorParameter"/>, this value is the constructor parameter name.
        /// </summary>
        [CanBeNull]
        string Name { get; }
        
        /// <summary>
        /// If the value is not null, represents the type of target for which a provided value is intended.<br/>
        /// - <see cref="IoC.Configuration.ProvidedValueTargetType.Setting"/>: Indicates the target is a configuration setting.<br/>
        /// - <see cref="IoC.Configuration.ProvidedValueTargetType.Property"/>: Indicates the target is a property.<br/>
        /// - <see cref="IoC.Configuration.ProvidedValueTargetType.ConstructorParameter"/>: Indicates the target is a constructor parameter.
        /// </summary>
        [CanBeNull]
        ProvidedValueTargetType? ProvidedValueTargetType { get; }
    }
}