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
    public class BindingNonGeneric : Binding, IBindingNonGeneric
    {
        #region  Constructors

        public BindingNonGeneric([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder,
                                 [NotNull] BindingConfigurationForCode bindingConfiguration) : base(serviceRegistrationBuilder, bindingConfiguration)
        {
        }

        #endregion

        #region IBindingNonGeneric Interface Implementation

        public IBindingNonGeneric OnlyIfNotRegistered()
        {
            BindingConfiguration.RegisterIfNotRegistered = true;
            return this;
        }

        public BindingImplementationNonGeneric To(Type implementationType)
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateTypeBasedImplementationConfiguration(BindingConfiguration.ServiceType, implementationType);
            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationNonGeneric(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        public BindingImplementationNonGeneric To(Func<IDiContainer, object> resolverFunc)
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateDelegateBasedImplementationConfiguration(BindingConfiguration.ServiceType, resolverFunc);
            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationNonGeneric(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        public IBindingImplementationNonGeneric ToSelf()
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateSelfImplementationConfiguration(BindingConfiguration.ServiceType);
            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationNonGeneric(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        #endregion
    }
}