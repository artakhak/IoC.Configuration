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
using System.Text;
using System.Xml;
using IoC.Configuration.DiContainerBuilder.FileBased;
using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    /// <summary>
    ///     Used for values created by constructor. For example the value might be defined by specifying type and constructor
    ///     parameters
    ///     using <see cref="ConfigurationFileElementNames.Parameters" />).
    ///     If no <see cref="ConfigurationFileElementNames.Parameters" /> child element exists, the type should have a default
    ///     constructor.
    /// </summary>
    public class ConstructedValueElement : ConstructedValueElementBase
    {
        #region Member Variables

        private Type[] _constructorParameterTypes;

        [NotNull]
        private readonly ICreateInstanceFromTypeAndConstructorParameters _createInstanceFromTypeAndConstructorParameters;

        private string _createInstanceMethodName;

        [NotNull]
        private readonly IInjectedPropertiesValidator _injectedPropertiesValidator;

        #endregion

        #region  Constructors

        public ConstructedValueElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                       [NotNull] ITypeHelper typeHelper,
                                       [NotNull] IImplementedTypeValidator implementedTypeValidator,
                                       [NotNull] IInjectedPropertiesValidator injectedPropertiesValidator,
                                       [NotNull] ICreateInstanceFromTypeAndConstructorParameters createInstanceFromTypeAndConstructorParameters) :
            base(xmlElement, parent, typeHelper, implementedTypeValidator)
        {
            _injectedPropertiesValidator = injectedPropertiesValidator;
            _createInstanceFromTypeAndConstructorParameters = createInstanceFromTypeAndConstructorParameters;
        }

        #endregion

        #region Member Functions

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IInjectedProperties injectedProperties)
                InjectedProperties = injectedProperties;
        }

        protected override void AddCodeOnGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            List<IInjectedPropertyElement> injectedProperties;

            if (InjectedProperties == null)
                injectedProperties = new List<IInjectedPropertyElement>();
            else
                injectedProperties = new List<IInjectedPropertyElement>(InjectedProperties.AllProperties);

            if (injectedProperties.Count > 0)
            {
                var dynamicImplementationsClass = dynamicAssemblyBuilder.GetDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.DynamicImplementationsClassName);

                var createInstanceMethodData = dynamicImplementationsClass.StartMethod("CreateInstance", ValueTypeInfo.Type, new IMethodParameterInfo[0], AccessLevel.Public, true, true);
                _createInstanceMethodName = createInstanceMethodData.MethodName;

                createInstanceMethodData.AddCodeLine("{");

                createInstanceMethodData.AddCode("var instance=");

                var cSharpCode = new StringBuilder();
                AddConstructorCode(dynamicAssemblyBuilder, cSharpCode);
                createInstanceMethodData.AddCode(cSharpCode.ToString());

                createInstanceMethodData.AddCodeLine(";");

                foreach (var injectedProperty in injectedProperties)
                    createInstanceMethodData.AddCodeLine($"instance.{injectedProperty.Name}={injectedProperty.GenerateValueCSharp(dynamicAssemblyBuilder)};");

                createInstanceMethodData.AddCodeLine("return instance;");

                createInstanceMethodData.AddCodeLine("}");
            }
        }

        private void AddConstructorCode([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder,
                                        [NotNull] StringBuilder cSharpCode)
        {
            cSharpCode.Append($"new {ValueTypeInfo.TypeCSharpFullName}(");

            if (Parameters != null)
            {
                var parameterIndex = 0;
                foreach (var parameterElement in Parameters.AllParameters)
                {
                    if (parameterIndex > 0)
                        cSharpCode.Append(", ");

                    cSharpCode.Append(parameterElement.GenerateValueCSharp(dynamicAssemblyBuilder));
                    ++parameterIndex;
                }
            }

            cSharpCode.Append(")");
        }

        protected override string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            var cSharpCode = new StringBuilder();

            if (_createInstanceMethodName != null)
            {
                var dynamicImplementationsClass = dynamicAssemblyBuilder.GetDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.DynamicImplementationsClassName);
                cSharpCode.Append($"{dynamicImplementationsClass.ClassFullName}.{_createInstanceMethodName}()");
            }
            else
            {
                AddConstructorCode(dynamicAssemblyBuilder, cSharpCode);
            }

            return cSharpCode.ToString();
        }

        public override object GenerateValue()
        {
            return _createInstanceFromTypeAndConstructorParameters.CreateInstance(this, ValueTypeInfo.Type, Parameters?.AllParameters, InjectedProperties?.AllProperties);
        }

        [CanBeNull]
        [ItemNotNull]
        protected IInjectedProperties InjectedProperties { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            if (Parameters != null && Parameters.AllParameters.Any())
                _constructorParameterTypes = Parameters.AllParameters.Select(x => x.ValueTypeInfo.Type).ToArray();
            else
                _constructorParameterTypes = new Type[0];

            if (!GlobalsCoreAmbientContext.Context.CheckTypeConstructorExistence(ValueTypeInfo.Type,
                _constructorParameterTypes,
                out var constructorInfo, out var errorMessage))
                throw new ConfigurationParseException(this, errorMessage);

            if (InjectedProperties != null)
            {
                _injectedPropertiesValidator.ValidateInjectedProperties(this, ValueTypeInfo.Type,
                    InjectedProperties.AllProperties, out var injectedPropertiesInfo);
            }
        }

        #endregion
    }
}