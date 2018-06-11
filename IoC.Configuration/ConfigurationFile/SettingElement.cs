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
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public class SettingElement : ParameterElement, ISettingElement
    {
        #region  Constructors

        public SettingElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                              [NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator,
                              [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent, typeBasedSimpleSerializerAggregator, assemblyLocator)
        {
        }

        #endregion

        #region ISettingElement Interface Implementation

        public override void Initialize()
        {
            base.Initialize();

            var assemblyAttributeValue = GetAttributeValue(ConfigurationFileAttributeNames.Assembly)?.Trim();
            
            if (!string.IsNullOrEmpty(assemblyAttributeValue))
            {
                var assemblySetting = Helpers.GetAssemblySettingByAssemblyAlias(this, assemblyAttributeValue);

                var parentPluginSetupElement = this.GetParentPluginSetupElement();

                if (parentPluginSetupElement == null)
                {
                    if (assemblySetting.OwningPluginElement != null)
                        throw new ConfigurationParseException(this, $"Assembly '{assemblySetting.Alias}' belongs to plugin  '{assemblySetting.OwningPluginElement.Name}' and cannot be used in general settings.");
                }
                else if (assemblySetting.OwningPluginElement != null && assemblySetting.OwningPluginElement != parentPluginSetupElement.Plugin)
                {
                    throw new ConfigurationParseException(this, $"Assembly '{assemblySetting.Alias}' belongs to plugin  '{assemblySetting.OwningPluginElement.Name}' and cannot be used in settings for plugin '{parentPluginSetupElement.Plugin.Name}'.");
                }
            }

            if (ValueInstantiationType == ValueInstantiationType.ResolveFromDiContext)
                throw new ConfigurationParseException(this, $"Settings cannot use '{ConfigurationFileElementNames.ValueInjectedObject}' element.");
        }

        #endregion
    }
}