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
using System.Reflection;
using OROptimizer;

namespace IoC.Configuration.ConfigurationFile
{
    public class InjectedPropertiesValidator : IInjectedPropertiesValidator
    {
        #region IInjectedPropertiesValidator Interface Implementation

        public void ValidateInjectedProperties(IConfigurationFileElement configurationFileElement,
                                               Type implementationType,
                                               IEnumerable<IInjectedPropertyElement> injectectedProperties,
                                               out IReadOnlyList<PropertyInfo> injectedPropertiesInfo)
        {
            var generatedInjectedPropertiesInfo = new List<PropertyInfo>();
            injectedPropertiesInfo = generatedInjectedPropertiesInfo;

            foreach (var injectedProperty in injectectedProperties)
            {
                PropertyInfo propertyInfo = null;
                try
                {
                    propertyInfo = implementationType.GetProperty(injectedProperty.Name);
                }
                catch (Exception)
                {
                    // We will throw later
                }

                if (propertyInfo == null || propertyInfo.GetSetMethod(false) == null)
                    throw new ConfigurationParseException(injectedProperty,
                        $"Type '{implementationType.FullName}' does not have a public setter property '{injectedProperty.Name}'.",
                        configurationFileElement);

                if (!propertyInfo.PropertyType.IsTypeAssignableFrom(injectedProperty.ValueTypeInfo.Type))
                    throw new ConfigurationParseException(injectedProperty,
                        $"The injected property '{injectedProperty.Name}' is of type '{injectedProperty.ValueTypeInfo.Type}' which is not assignable to type of property '{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}'. Property '{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}' is of type '{propertyInfo.PropertyType.FullName}'.",
                        configurationFileElement);

                if (propertyInfo.GetIndexParameters()?.Length > 0)
                    throw new ConfigurationParseException(injectedProperty, $"Property '{propertyInfo.Name}' in type '{propertyInfo.DeclaringType.FullName}' has index parameters. Injected properties with index parameters are currently not supported in '{ConfigurationFileElementNames.InjectedProperties}' element.", configurationFileElement);

                generatedInjectedPropertiesInfo.Add(propertyInfo);
            }
        }

        #endregion
    }
}