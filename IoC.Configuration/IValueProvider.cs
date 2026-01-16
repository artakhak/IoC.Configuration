using JetBrains.Annotations;

namespace IoC.Configuration
{
    /// <summary>
    /// Defines a mechanism for providing and resolving values dynamically within a configuration file structure.
    /// </summary>
    public interface IValueProvider
    {
        /// <summary>
        /// Attempts to resolve a value dynamically within a configuration file structure.
        /// </summary>
        /// <param name="providedValueData">
        /// An object representing the provided value data used for resolving and initializing a value.
        /// </param>
        /// <param name="resolvedValue">
        /// When this method returns, contains the resolved value if the resolution was successful, or null if the resolution failed.
        /// </param>
        /// <returns>
        /// true if the value was successfully resolved; otherwise, false.
        /// </returns>
        bool TryResolveValue(IProvidedValueData providedValueData, [CanBeNull] out object resolvedValue);
    }
}