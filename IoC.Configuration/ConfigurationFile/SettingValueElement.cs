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

using System.Xml;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public class SettingValueInitializerHelper : ISettingValueInitializerHelper
    {
        #region ISettingValueInitializerHelper Interface Implementation

        public ISettingElement GetSettingElement(IConfigurationFileElement requestingConfigurationFileElement, string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
                throw new ConfigurationParseException(requestingConfigurationFileElement, "The setting name cannot be empty.");

            ISettingElement _settingElement = null;
            if (requestingConfigurationFileElement.OwningPluginElement != null)
                _settingElement = requestingConfigurationFileElement.GetPluginSetupElement().SettingsElement?.GetSettingElement(settingName);

            if (_settingElement == null)
                _settingElement = requestingConfigurationFileElement.Configuration.SettingsElement?.GetSettingElement(settingName);

            if (_settingElement == null)
                throw new ConfigurationParseException(requestingConfigurationFileElement, $"Setting with name '{settingName}' was not found.");

            return _settingElement;
        }

        #endregion
    }

    public class SettingValueElement : ValueInitializerElement
    {
        #region Member Variables

        private ISettingElement _settingElement;

        [NotNull]
        private readonly ISettingValueInitializerHelper _settingValueInitializerHelper;

        #endregion

        #region  Constructors

        public SettingValueElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                   [NotNull] ITypeHelper typeHelper,
                                   [NotNull] ISettingValueInitializerHelper settingValueInitializerHelper) : base(xmlElement, parent, typeHelper)
        {
            _settingValueInitializerHelper = settingValueInitializerHelper;
        }

        #endregion

        #region Member Functions

        /// <summary>
        ///     Generates a code that returns an instance of a value of type specified by property
        ///     <see cref="P:IoC.Configuration.IValueInitializer.ValueType" />.
        /// </summary>
        /// <param name="dynamicAssemblyBuilder">The dynamic assembly builder.</param>
        /// <returns></returns>
        protected override string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            return _settingElement.GenerateValueCSharp(dynamicAssemblyBuilder);
        }

        /// <summary>
        ///     Generates the value using reflection. Use this value only at early stages of loading the configuration,
        ///     when the DI container is not yet initialized.
        /// </summary>
        /// <returns></returns>
        public override object GenerateValue()
        {
            return _settingElement.DeserializedValue;
        }

        protected override ITypeInfo GetValueTypeInfo()
        {
            return _settingElement.ValueTypeInfo;
        }

        public override void Initialize()
        {
            InitializeSettingData();

            base.Initialize();
        }

        private void InitializeSettingData()
        {
            var settingName = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.SettingName);
            _settingElement = _settingValueInitializerHelper.GetSettingElement(this, settingName);
        }

        /// <summary>Gets a value indicating whether this instance is resolved from di container.</summary>
        public override bool IsResolvedFromDiContainer => false;

        #endregion
    }
}