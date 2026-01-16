namespace IoC.Configuration
{
    /// <summary>
    /// Indicates that the provided value is intended for a configuration setting in the IoC framework.
    /// </summary>
    public enum ProvidedValueTargetType
    {
        /// <summary>
        /// This member specifies that the target for the value being resolved is a constructor parameter. It is
        /// used in scenarios where dependencies are injected into class constructors.
        /// </summary>
        ConstructorParameter,

        /// <summary>
        /// This member specifies that the target for the value being resolved is a property in the IoC configuration.
        /// It is used in scenarios where dependencies or settings are injected into properties of a class.
        /// </summary>
        Property,

        /// <summary>
        /// This member indicates that the provided value is intended for use as a configuration setting.
        /// </summary>
        Setting
    }
}