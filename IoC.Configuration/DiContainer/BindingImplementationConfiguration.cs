// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
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
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingImplementationConfiguration"/> class.
        /// </summary>
        /// <param name="targetImplementationType">Type of the target implementation.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        protected BindingImplementationConfiguration(TargetImplementationType targetImplementationType,
                                                     [CanBeNull] Type implementationType)
        {
            TargetImplementationType = targetImplementationType;
            ImplementationType = implementationType;
        }

        /// <summary>
        /// Gets the type of the target implementation.
        /// </summary>
        /// <value>
        /// The type of the target implementation.
        /// </value>
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

        /// <summary>
        /// Gets the type of the implementation.
        /// </summary>
        /// <value>
        /// The type of the implementation.
        /// </value>
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

        /// <summary>
        /// Checks the value not null and set once.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        protected void CheckValueNotNullAndSetOnce([NotNull] string propertyName, [CanBeNull] object value)
        {
            GlobalsCoreAmbientContext.Context.EnsureParameterNotNull($"{GetType().FullName}.{propertyName}", value);

            if (_setPropertyNames.Contains(propertyName))
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                    $"The value of '{GetType().FullName}.{nameof(propertyName)}' can be set only once.");

            _setPropertyNames.Add(propertyName);
        }

        /// <summary>
        /// Validates the binding configuration
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