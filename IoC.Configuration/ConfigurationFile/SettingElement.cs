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
using IoC.Configuration.DiContainerBuilder.FileBased;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public class SettingElement : ParameterElement, ISettingElement
    {
        #region Member Variables

        [NotNull]
        private readonly IIdentifierValidator _identifierValidator;

        #endregion

        #region  Constructors

        public SettingElement([NotNull] IValueInitializerElement decoratedValueInitializerElement,
                              [NotNull] IIdentifierValidator identifierValidator) : base(decoratedValueInitializerElement)
        {
            _identifierValidator = identifierValidator;
        }

        #endregion

        #region ISettingElement Interface Implementation

        public object DeserializedValue { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            _identifierValidator.Validate(this, ConfigurationFileAttributeNames.Name, Name);

            if (IsResolvedFromDiContainer)
                throw new ConfigurationParseException(this, $"Settings cannot use '{ConfigurationFileElementNames.ValueInjectedObject}' element.");
        }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();
            DeserializedValue = GenerateValue();
        }
        #endregion

        #region Member Functions

        protected override void AddCodeOnGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            var dynamicallyGeneratedClass = dynamicAssemblyBuilder.GetDynamicallyGeneratedClass(GetSettingsClassName());
            dynamicallyGeneratedClass.AddCodeLine($"public static {ValueTypeInfo.TypeCSharpFullName} {DynamicCodeGenerationHelpers.GetSettingValuePropertyName(Name)} {{ get; }} = {DecoratedValueInitializerElement.GenerateValueCSharp(dynamicAssemblyBuilder)};");
        }

        protected override string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            var dynamicallyGeneratedClass = dynamicAssemblyBuilder.GetDynamicallyGeneratedClass(GetSettingsClassName());
            return $"{dynamicallyGeneratedClass.ClassFullName}.{DynamicCodeGenerationHelpers.GetSettingValuePropertyName(Name)}";
        }

        private string GetSettingsClassName()
        {
            return OwningPluginElement == null ? DynamicCodeGenerationHelpers.SettingValuesClassName : DynamicCodeGenerationHelpers.GetPluginSettingValuesClassName(OwningPluginElement.Name);
        }

        #endregion
    }
}