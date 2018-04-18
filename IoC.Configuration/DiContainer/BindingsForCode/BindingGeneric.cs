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
    public class BindingGeneric<TService> : Binding, IBindingGeneric<TService>
    {
        #region  Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingGeneric{TService}"/> class.
        /// </summary>
        /// <param name="serviceRegistrationBuilder">The service registration builder.</param>
        /// <param name="bindingConfiguration">The binding configuration.</param>
        public BindingGeneric([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder,
                              [NotNull] BindingConfigurationForCode bindingConfiguration) : base(serviceRegistrationBuilder, bindingConfiguration)
        {
        }

        #endregion

        #region IBindingGeneric<TService> Interface Implementation
        /// <summary>
        /// If called, the binding will be registered only if binding for the service has not been already registered.
        /// Note, multiple implementations can still be registered using this binding, if the binding for service was not registered.
        /// </summary>
        /// <returns>
        /// Returns <see cref="T:IoC.Configuration.DiContainer.BindingsForCode.IBindingGeneric`1" />
        /// </returns>
        public IBindingGeneric<TService> OnlyIfNotRegistered()
        {
            BindingConfiguration.RegisterIfNotRegistered = true;
            return this;
        }

        /// <summary>
        /// Bind service <typeparamref name="TService" /> to implementation <typeparamref name="TImplementation" />.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <returns>
        /// Returns <see cref="T:IoC.Configuration.DiContainer.BindingsForCode.IBindingImplementationGeneric`2" />
        /// </returns>
        public IBindingImplementationGeneric<TService, TImplementation> To<TImplementation>() where TImplementation : TService
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateTypeBasedImplementationConfiguration(BindingConfiguration.ServiceType, typeof(TImplementation));
            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationGeneric<TService, TImplementation>(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        /// <summary>
        /// Bind service <typeparamref name="TService" /> to the result of function call <paramref name="resolverFunc" />(<see cref="T:IoC.Configuration.DiContainer.IDiContainer" />).
        /// </summary>
        /// <param name="resolverFunc">The resolver function.</param>
        /// <returns>
        /// Returns <see cref="T:IoC.Configuration.DiContainer.BindingsForCode.IBindingImplementationGeneric`2" />
        /// </returns>
        public IBindingImplementationGeneric<TService, TService> To(Func<IDiContainer, TService> resolverFunc)
        {
            return To<TService>(resolverFunc);
        }

        /// <summary>
        /// Bind service <typeparamref name="TService" /> to the result of function call <paramref name="resolverFunc" />(<see cref="T:IoC.Configuration.DiContainer.IDiContainer" />).
        /// TImplementation should be either <typeparamref name="TService" />, or a type that implements or derives from
        /// <typeparamref name="TService" />.
        /// Note, we can use value of <typeparamref name="TService" /> for <typeparamref name="TImplementation" /> not to restrict what the function
        /// <paramref name="resolverFunc" /> returns. In other words to enforce that the implementation returns <typeparamref name="TService" />
        /// or a subclass or implementation of <typeparamref name="TService" />.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="resolverFunc">The resolver function.</param>
        /// <returns>
        /// Returns <see cref="T:IoC.Configuration.DiContainer.BindingsForCode.IBindingImplementationGeneric`2" />
        /// </returns>
        public IBindingImplementationGeneric<TService, TImplementation> To<TImplementation>(Func<IDiContainer, TImplementation> resolverFunc) where TImplementation : TService
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateDelegateBasedImplementationConfiguration(BindingConfiguration.ServiceType,
                typeResolver => resolverFunc(typeResolver));

            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationGeneric<TService, TImplementation>(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        /// <summary>
        /// Bind service <typeparamref name="TService" /> to itself.
        /// </summary>
        /// <returns>
        /// Returns <see cref="T:IoC.Configuration.DiContainer.BindingsForCode.IBindingImplementationGeneric`2" />
        /// </returns>
        public IBindingImplementationGeneric<TService, TService> ToSelf()
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateSelfImplementationConfiguration(BindingConfiguration.ServiceType);
            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationGeneric<TService, TService>(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        #endregion
    }
}