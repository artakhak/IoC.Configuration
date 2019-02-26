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

using System.Collections.Generic;
using System.Linq;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public sealed class TypeBasedBindingImplementationConfigurationForFile : BindingImplementationConfigurationForFile, ITypeBasedBindingImplementationConfigurationForFile
    {
        #region  Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BindingImplementationConfigurationForFile" /> class.
        /// </summary>
        /// <param name="serviceImplementationElement">The service implementation element.</param>
        /// <exception cref="System.Exception">
        /// </exception>
        public TypeBasedBindingImplementationConfigurationForFile([NotNull] ITypeBasedServiceImplementationElement serviceImplementationElement) :
            base(serviceImplementationElement)
        {
            if (serviceImplementationElement.Parameters != null)
            {
                var parameters = new LinkedList<IParameter>();

                foreach (var parameter in serviceImplementationElement.Parameters.AllParameters)
                    parameters.AddLast(new Parameter(parameter));

                Parameters = parameters.ToArray();
            }

            if (serviceImplementationElement.InjectedProperties != null)
            {
                var injectedProperties = new LinkedList<IInjectedProperty>();

                foreach (var injectedProperty in serviceImplementationElement.InjectedProperties.AllProperties)
                    injectedProperties.AddLast(new InjectedProperty(injectedProperty));

                InjectedProperties = injectedProperties;
            }
        }

        #endregion

        #region ITypeBasedBindingImplementationConfigurationForFile Interface Implementation

        /// <inheritdoc />
        [CanBeNull]
        public IEnumerable<IInjectedProperty> InjectedProperties { get; }

        /// <inheritdoc />
        [CanBeNull]
        [ItemNotNull]
        public IParameter[] Parameters { get; }

        #endregion
    }
}