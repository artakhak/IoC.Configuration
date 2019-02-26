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
using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    public class CreateInstanceFromTypeAndConstructorParameters : ICreateInstanceFromTypeAndConstructorParameters
    {
        #region Member Variables

        [NotNull]
        private readonly IInjectedPropertiesValidator _injectedPropertiesValidator;

        #endregion

        #region  Constructors

        public CreateInstanceFromTypeAndConstructorParameters([NotNull] IInjectedPropertiesValidator injectedPropertiesValidator)
        {
            _injectedPropertiesValidator = injectedPropertiesValidator;
        }

        #endregion

        #region ICreateInstanceFromTypeAndConstructorParameters Interface Implementation

        public object CreateInstance(IConfigurationFileElement configurationFileElement, Type createdObjectType,
                                     IEnumerable<IParameterElement> constructorParameters,
                                     IEnumerable<IInjectedPropertyElement> injectedProperties = null)
        {
            return GenerateValueLocal(configurationFileElement, null, createdObjectType, constructorParameters, injectedProperties);
        }

        public object CreateInstance(IConfigurationFileElement configurationFileElement, Type validBaseType, Type createdObjectType,
                                     IEnumerable<IParameterElement> constructorParameters,
                                     IEnumerable<IInjectedPropertyElement> injectedProperties = null)
        {
            return GenerateValueLocal(configurationFileElement, validBaseType, createdObjectType, constructorParameters, injectedProperties);
        }

        #endregion

        #region Member Functions

        private object GenerateValueLocal(IConfigurationFileElement configurationFileElement, Type validBaseType, Type createdObjectType,
                                          IEnumerable<IParameterElement> constructorParameters,
                                          IEnumerable<IInjectedPropertyElement> injectedProperties)
        {
            var constructorParametersArray = constructorParameters == null ? new IParameterElement[0] : constructorParameters.ToArray();

            var parameterInfos = new ParameterInfo[constructorParametersArray.Length];

            try
            {
                for (var parameterIndex = 0; parameterIndex < constructorParametersArray.Length; ++parameterIndex)
                {
                    var parameter = constructorParametersArray[parameterIndex];

                    try
                    {
                        parameterInfos[parameterIndex] = new ParameterInfo(parameter.ValueTypeInfo.Type, parameter.GenerateValue());
                    }
                    catch (Exception e)
                    {
                        LogHelper.Context.Log.Error(e);
                        throw new ConfigurationParseException(parameter, $"Failed to generate parameter value for parameter {parameter.Name}.");
                    }
                }

                object generatedInstance;

                string errorMessage;
                if (validBaseType == null)
                    generatedInstance = GlobalsCoreAmbientContext.Context.CreateInstance(createdObjectType, parameterInfos, out errorMessage);
                else
                    generatedInstance = GlobalsCoreAmbientContext.Context.CreateInstance(validBaseType, createdObjectType, parameterInfos, out errorMessage);

                if (generatedInstance == null)
                {
                    if (errorMessage == null)
                        errorMessage = $"Failed to generate an instance of type {createdObjectType}.";

                    throw new ConfigurationParseException(configurationFileElement, errorMessage);
                }

                if (injectedProperties != null)
                {
                    var injectedPropertiesArray = injectedProperties.ToArray();

                    _injectedPropertiesValidator.ValidateInjectedProperties(configurationFileElement, createdObjectType, injectedProperties,
                        out var injectedPropertyInfos);

                    for (var i = 0; i < injectedPropertiesArray.Length; ++i)
                    {
                        var injectedProperty = injectedPropertiesArray[i];

                        var propertyInfo = injectedPropertyInfos.Count <= i ? null : injectedPropertyInfos[i];

                        if (propertyInfo == null)
                            throw new ConfigurationParseException(configurationFileElement, $"Property '{createdObjectType.FullName}.{injectedProperty.Name}' was not found. This error should never happen unless something is wrong in IoC.Configuration parser.");

                        propertyInfo.SetValue(generatedInstance, injectedProperty.GenerateValue());
                    }
                }

                return generatedInstance;
            }
            catch (Exception e) when (!(e is ConfigurationParseException))
            {
                LogHelper.Context.Log.Error(e);
                throw new ConfigurationParseException(configurationFileElement, $"Failed to generate an instance of type {createdObjectType}.");
            }
        }

        #endregion
    }
}