using System;

namespace IoC.Configuration.DiContainer
{
    // Non standard features will not be used to avoid situations when the configuration file works for one container, but not the other.
    // If non-standard features are needed, use native module (e.g., Autofac or Ninject module).
    [Obsolete]
    public enum NonStandardFeatures
    {
        /// <summary>
        ///     Allows registering a service, only if the service is not registered, when there is only one implementation for the
        ///     service.
        /// </summary>
        RegisterServiceWithSingleImplementationsOnlyIfServiceNotRegistered,

        /// <summary>
        ///     Allows registering a service, only if the service is not registered, when multiple implementations are registered
        ///     for the service.
        ///     The specific IoC container (e.g., Autofac, Ninject, etc),
        ///     should support registering multiple implementations of the service, if the service was not registered.
        ///     For example Autofac currently supports this for single implementation (using IfNotRegistered(Type)), but there is
        ///     no way to do this for
        ///     multiple implementations of a service (i.e., check if service was registered, and register all implementations if
        ///     it was not).
        /// </summary>
        RegisterServiceWithMultipleImplementationsOnlyIfServiceNotRegistered,

        /// <summary>
        ///     Specific implementation will be applied only if the implementation is injected into a specific type or its
        ///     subclasses.
        /// </summary>
        WhenInjectedIntoClassOrSubClassesConditionalInjection,

        /// <summary>
        ///     Specific implementation will be applied only if the implementation is injected into a specific type.
        /// </summary>
        WhenInjectedExactlyIntoClassConditionalInjection
    }
}