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
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public class BindingImplementationGeneric<TService, TImplementation> : BindingImplementation, IBindingImplementationGeneric<TService, TImplementation>
    {
        #region  Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BindingImplementationGeneric{TService, TImplementation}" /> class.
        /// </summary>
        /// <param name="serviceRegistrationBuilder">The service registration builder.</param>
        /// <param name="bindingImplementationConfiguration">The binding implementation configuration.</param>
        /// <param name="serviceBinding">The service binding.</param>
        public BindingImplementationGeneric([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder,
                                            [NotNull] BindingImplementationConfigurationForCode bindingImplementationConfiguration,
                                            [NotNull] BindingGeneric<TService> serviceBinding) : base(serviceRegistrationBuilder, bindingImplementationConfiguration)
        {
            Service = serviceBinding;
        }

        #endregion

        #region IBindingImplementationGeneric<TService,TImplementation> Interface Implementation

        /// <summary>
        ///     React to an event that occurs when the implementation is instantiated.
        /// </summary>
        /// <param name="onImplementationActivated">The on implementation activated.</param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="T:IoC.Configuration.DiContainer.BindingsForCode.IBindingImplementationGeneric`2" />
        /// </returns>
        public IBindingImplementationGeneric<TService, TImplementation> OnImplementationObjectActivated(Action<IDiContainer, TImplementation> onImplementationActivated)
        {
            BindingImplementationConfiguration.OnImplementationObjectActivated =
                (typeResolver, implementationObject) => onImplementationActivated(typeResolver, (TImplementation) implementationObject);
            return this;
        }

        /// <summary>
        ///     Use this member to add multiple implementations for the same service.
        /// </summary>
        [NotNull]
        public IBindingGeneric<TService> Service { get; }

        /// <summary>
        ///     Sets the resolution scope.
        /// </summary>
        /// <param name="resolutionScope">The resolution scope.</param>
        /// <returns>
        ///     Returns an instance of
        ///     <see cref="T:IoC.Configuration.DiContainer.BindingsForCode.IBindingImplementationGeneric`2" />
        /// </returns>
        public IBindingImplementationGeneric<TService, TImplementation> SetResolutionScope(DiResolutionScope resolutionScope)
        {
            BindingImplementationConfiguration.ResolutionScope = resolutionScope;
            return this;
        }

        #endregion

#if DEBUG
// Will enable this code in release mode when Autofac implementation for this feature is available.
        //public IBindingImplementationGeneric<TService, TImplementation> WhenInjectedInto(Type targetType, bool considerAlsoTargetTypeSubclasses)
        //{
        //    BindingImplementationConfiguration.SetWhenInjectedIntoData(
        //        considerAlsoTargetTypeSubclasses ? ConditionalInjectionType.WhenInjectedInto :
        //        ConditionalInjectionType.WhenInjectedExactlyInto, targetType);
        //    return this;
        //}

        //public IBindingImplementationGeneric<TService, TImplementation> WhenInjectedInto<T>(bool considerAlsoTargetTypeSubclasses)
        //{
        //    BindingImplementationConfiguration.SetWhenInjectedIntoData(
        //        considerAlsoTargetTypeSubclasses ? ConditionalInjectionType.WhenInjectedInto :
        //            ConditionalInjectionType.WhenInjectedExactlyInto, typeof(T));
        //    return this;
        //} 
#endif
    }
}