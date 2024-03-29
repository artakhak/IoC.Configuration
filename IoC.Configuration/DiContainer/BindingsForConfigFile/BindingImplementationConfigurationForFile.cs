﻿// This software is part of the IoC.Configuration library
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
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public abstract class BindingImplementationConfigurationForFile : BindingImplementationConfiguration
    {
        #region  Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BindingImplementationConfigurationForFile" /> class.
        /// </summary>
        /// <param name="serviceToProxyImplementationElement">The service implementation element.</param>
        /// <exception cref="System.Exception">
        /// </exception>
        protected BindingImplementationConfigurationForFile([NotNull] IServiceImplementationElement serviceToProxyImplementationElement)
            : base(GetTargetImplementationType(serviceToProxyImplementationElement), serviceToProxyImplementationElement.ValueTypeInfo.Type)
        {
            ResolutionScope = serviceToProxyImplementationElement.ResolutionScope;

            if (!serviceToProxyImplementationElement.Enabled)
                throw new Exception($"The value of '{serviceToProxyImplementationElement}.{nameof(IServiceImplementationElement.Enabled)}' cannot be false.");

#if DEBUG
// Will enable this code in release mode when Autofac implementation for this feature is available.
            //if (serviceImplementationElement.ConditionalInjectionType != ConditionalInjectionType.None)
            //    SetWhenInjectedIntoData(serviceImplementationElement.ConditionalInjectionType, serviceImplementationElement.WhenInjectedIntoType); 
#endif
        }

        #endregion

        #region Member Functions

        private static TargetImplementationType GetTargetImplementationType([NotNull] IServiceImplementationElement serviceImplementationElement)
        {
            if (serviceImplementationElement is ISelfBoundServiceElement)
                return TargetImplementationType.Self;

            if (serviceImplementationElement is IServiceToProxyImplementationElement)
                return TargetImplementationType.ProxiedType;

            return TargetImplementationType.Type;
        }

        public override string ToString()
        {
            return $"{GetType().FullName}, TargetImplementationType :{TargetImplementationType}, ImplementationType: {ImplementationType}.";
        }

        #endregion
    }
}