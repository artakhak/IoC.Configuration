using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.DiContainer
{
    public class BindingImplementationConfiguration
    {
        [NotNull]
        [ItemNotNull]
        private readonly HashSet<string> _setPropertyNames = new HashSet<string>(StringComparer.Ordinal);


        [CanBeNull]
        private DiResolutionScope? _resolutionScope;

        protected BindingImplementationConfiguration(TargetImplementationType targetImplementationType,
                                                     [CanBeNull] Type implementationType)
        {
            TargetImplementationType = targetImplementationType;
            ImplementationType = implementationType;
        }

        public TargetImplementationType TargetImplementationType { get; }

        /// <summary>
        ///     If not set, will default to <see cref="DiResolutionScope.Singleton" />.
        /// </summary>
        [CanBeNull]
        public DiResolutionScope ResolutionScope
        {
            get => _resolutionScope ?? DiResolutionScope.Singleton;
            set
            {
                CheckValueNotNullAndSetOnce(nameof(ResolutionScope), value);
                _resolutionScope = value;
            }
        }

        [CanBeNull]
        public Type ImplementationType { get; }

        // Autofac: https://stackoverflow.com/questions/7664912/autofac-equivalent-of-ninjects-wheninjectedinto
        [CanBeNull]
        public Type WhenInjectedIntoType { get; private set; }

        public ConditionalInjectionType ConditionalInjectionType { get; } = ConditionalInjectionType.None;

//#if DEBUG
//// Will enable this code in release mode when Autofac implementation for this feature is available.
//        public void SetWhenInjectedIntoData(ConditionalInjectionType conditionalInjectionType, Type injectionTargetType)
//        {
//            CheckValueNotNullAndSetOnce(nameof(WhenInjectedIntoType), injectionTargetType);

//            ConditionalInjectionType = conditionalInjectionType;
//            WhenInjectedIntoType = injectionTargetType;
//        } 
//#endif

        protected void CheckValueNotNullAndSetOnce([NotNull] string propertyName, [CanBeNull] object value)
        {
            GlobalsCoreAmbientContext.Context.EnsureParameterNotNull($"{GetType().FullName}.{propertyName}", value);

            if (_setPropertyNames.Contains(propertyName))
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                    $"The value of '{GetType().FullName}.{nameof(propertyName)}' can be set only once.");

            _setPropertyNames.Add(propertyName);
        }

        /// <summary>
        /// </summary>
        /// <exception cref="Exception">
        ///     Throws an exception if service binding data is invalid. Example of invalid data is invalid
        ///     implementation type.
        /// </exception>
        public virtual void Validate()
        {
            if (ConditionalInjectionType == ConditionalInjectionType.None != (WhenInjectedIntoType == null))
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"If '{GetType().FullName}.{nameof(ConditionalInjectionType)}' is '{ConditionalInjectionType.None}' then the value of '{GetType().FullName}.{nameof(WhenInjectedIntoType)}' should be null. Otherwise, '{GetType().FullName}.{nameof(WhenInjectedIntoType)}' cannot be null.");
        }
    }
}