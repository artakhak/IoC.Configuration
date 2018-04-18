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
using System.Linq;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public sealed class BindingImplementationConfigurationForFile : BindingImplementationConfiguration
    {
        #region  Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingImplementationConfigurationForFile"/> class.
        /// </summary>
        /// <param name="serviceImplementationElement">The service implementation element.</param>
        /// <exception cref="System.Exception">
        /// </exception>
        public BindingImplementationConfigurationForFile([NotNull] IServiceImplementationElement serviceImplementationElement)
            : base(GetTargetImplementationType(serviceImplementationElement), serviceImplementationElement.ImplementationType)
        {
            ResolutionScope = serviceImplementationElement.ResolutionScope;

            if (!serviceImplementationElement.Enabled)
                throw new Exception($"The value of '{serviceImplementationElement}.{nameof(IServiceImplementationElement.Enabled)}' cannot be null.");

            if (serviceImplementationElement.Parameters != null)
            {
                if (!serviceImplementationElement.Parameters.Enabled)
                    throw new Exception($"The value of '{serviceImplementationElement}.{nameof(IServiceImplementationElement.Parameters)}.{nameof(IConfigurationFileElement.Enabled)}' cannot be null.");

                var parameters = new LinkedList<IParameter>();

                foreach (var parameter in serviceImplementationElement.Parameters.AllParameters)
                    parameters.AddLast(new Parameter(parameter));

                Parameters = parameters.ToArray();
            }

            if (serviceImplementationElement.InjectedProperties != null)
            {
                if (!serviceImplementationElement.InjectedProperties.Enabled)
                    throw new Exception($"The value of '{serviceImplementationElement}.{nameof(IServiceImplementationElement.InjectedProperties)}.{nameof(IConfigurationFileElement.Enabled)}' cannot be null.");

                var injectedProperties = new LinkedList<IInjectedProperty>();

                foreach (var injectedProperty in serviceImplementationElement.InjectedProperties.AllProperties)
                    injectedProperties.AddLast(new InjectedProperty(injectedProperty));

                InjectedProperties = injectedProperties;
            }

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

            return TargetImplementationType.Type;
        }


        /// <summary>
        /// Gets the injected properties.
        /// </summary>
        /// <value>
        /// The injected properties.
        /// </value>
        [CanBeNull]
        public IEnumerable<IInjectedProperty> InjectedProperties { get; }

        /// <summary>
        ///     If the value is null, the parameters will be injected.
        ///     Otherwise, a constructor which matches the parameters by type and name will
        ///     be used to create an implementation.
        /// </summary>
        [CanBeNull, ItemNotNull]
        public IParameter[] Parameters { get; }

        public override string ToString()
        {
            return $"{GetType().FullName}, TargetImplementationType :{TargetImplementationType}, ImplementationType: {ImplementationType}.";
        }

        #endregion
    }
}