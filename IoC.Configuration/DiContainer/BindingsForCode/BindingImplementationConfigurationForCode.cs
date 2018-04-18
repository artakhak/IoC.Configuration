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
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public sealed class BindingImplementationConfigurationForCode : BindingImplementationConfiguration
    {
        #region Member Variables

        [CanBeNull]
        private Action<IDiContainer, object> _onImplementationObjectActivated;

        #endregion

        #region  Constructors

        private BindingImplementationConfigurationForCode([NotNull] Type serviceType, TargetImplementationType targetImplementationType, [CanBeNull] Type implementationType, [CanBeNull] Func<IDiContainer, object> implementationGeneratorFunction) :
            base(targetImplementationType, implementationType)
        {
            GlobalsCoreAmbientContext.Context.EnsureParameterNotNull(nameof(serviceType), serviceType);

            switch (TargetImplementationType)
            {
                case TargetImplementationType.Self:
                case TargetImplementationType.Type:

                    if (implementationType == null)
                        GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                            string.Format("The value of '{0}' cannot be null if '{1}' is '{2}'.",
                                nameof(implementationType), nameof(targetImplementationType),
                                targetImplementationType));

                    if (!serviceType.IsAssignableFrom(implementationType) ||
                        implementationType.IsAbstract || implementationType.IsInterface ||
                        implementationType.GetConstructors().FirstOrDefault(x => x.IsPublic) == null)
                    {
                        var errorMessageStrBldr = new StringBuilder();

                        errorMessageStrBldr.AppendLine($"Implementation '{implementationType}' specified for service '{serviceType.FullName}' is invalid.");
                        errorMessageStrBldr.AppendLine("The service type should be assignable from implementation type and the implementation type should be an instantiable class (e.g., not an interface or an abstract class).");
                        errorMessageStrBldr.AppendLine("Also, the implementation type should have at least one public constructor.");

                        GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(errorMessageStrBldr.ToString());
                    }

                    break;

                case TargetImplementationType.Delegate:
                    if (implementationGeneratorFunction == null)
                        GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                            string.Format("The value of '{0}' cannot be null if '{1}' is '{2}'.",
                                nameof(implementationGeneratorFunction), nameof(targetImplementationType),
                                targetImplementationType));

                    ImplementationGeneratorFunction = implementationGeneratorFunction;
                    break;
            }

            switch (TargetImplementationType)
            {
                case TargetImplementationType.Delegate:
                    if (implementationGeneratorFunction == null)
                        GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                            string.Format("The value of '{0}' cannot be null if '{1}' is '{2}'.",
                                nameof(implementationGeneratorFunction), nameof(targetImplementationType),
                                targetImplementationType));

                    ImplementationGeneratorFunction = implementationGeneratorFunction;
                    break;
            }
        }

        #endregion

        #region Member Functions        
        /// <summary>
        /// Creates the delegate based implementation configuration.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationGeneratorFunction">The implementation generator function.</param>
        /// <returns></returns>
        public static BindingImplementationConfigurationForCode CreateDelegateBasedImplementationConfiguration([NotNull] Type serviceType, [NotNull] Func<IDiContainer, object> implementationGeneratorFunction)
        {
            return new BindingImplementationConfigurationForCode(serviceType, TargetImplementationType.Delegate, null, implementationGeneratorFunction);
        }

        /// <summary>
        /// Creates the self implementation configuration.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public static BindingImplementationConfigurationForCode CreateSelfImplementationConfiguration([NotNull] Type serviceType)
        {
            return new BindingImplementationConfigurationForCode(serviceType, TargetImplementationType.Self, serviceType, null);
        }

        /// <summary>
        /// Creates the type based implementation configuration.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <returns></returns>
        public static BindingImplementationConfigurationForCode CreateTypeBasedImplementationConfiguration([NotNull] Type serviceType, [NotNull] Type implementationType)
        {
            return new BindingImplementationConfigurationForCode(serviceType, TargetImplementationType.Type, implementationType, null);
        }

        /// <summary>
        /// Gets the implementation generator function.
        /// </summary>
        /// <value>
        /// The implementation generator function.
        /// </value>
        [CanBeNull]
        public Func<IDiContainer, object> ImplementationGeneratorFunction { get; }

        /// <summary>
        ///     Will be called after the implementation object is activated.
        ///     Use this for property injection.
        ///     The value of <see cref="object" /> parameter is the activated implementation instance.
        /// </summary>
        [CanBeNull]
        public Action<IDiContainer, object> OnImplementationObjectActivated
        {
            get => _onImplementationObjectActivated;
            set
            {
                CheckValueNotNullAndSetOnce(nameof(OnImplementationObjectActivated), value);
                _onImplementationObjectActivated = value;
            }
        }

        #endregion
    }
}