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

namespace IoC.Configuration.ConfigurationFile
{
    public class PluginSetup : ConfigurationFileElementAbstr, IPluginSetup
    {
        #region Member Variables

        private bool? _isPluginImplementationEnabled;

        #endregion

        #region  Constructors

        public PluginSetup([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IPluginSetup Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IPluginImplementationElement)
            {
                PluginImplementationElement = (IPluginImplementationElement) child;
                _isPluginImplementationEnabled = child.Enabled;
            }
            else if (child is ISettingsElement)
            {
                SettingsElement = (ISettingsElement) child;
            }
            else if (child is IWebApi)
            {
                WebApi = (IWebApi)child;
            }
            else if (child is IDependencyInjection)
            {
                DependencyInjection = (IDependencyInjection) child;
            }
        }

        public IDependencyInjection DependencyInjection { get; private set; }

        public override bool Enabled => base.Enabled && (_isPluginImplementationEnabled ?? true);

        public override void Initialize()
        {
            base.Initialize();

            var pluginName = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Plugin);

            Plugin = _configuration.Plugins?.GetPlugin(pluginName);
            if (Plugin == null)
                throw new ConfigurationParseException(this, $"No plugin with name '{pluginName}' is defined in element '<{ConfigurationFileElementNames.RootElement}><{ConfigurationFileElementNames.Plugins}>'.");
        }

        public override IPluginElement OwningPluginElement => Plugin;

        public IPluginElement Plugin { get; private set; }
        public IPluginImplementationElement PluginImplementationElement { get; private set; }
        public ISettingsElement SettingsElement { get; private set; }

        public IWebApi WebApi { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            if (PluginImplementationElement == null)
                throw new ConfigurationParseException(this, $"The value of '{GetType().FullName}.{nameof(PluginImplementationElement)}' cannot be null.");

            if (!PluginImplementationElement.Assembly.Enabled && Plugin.Enabled)
                throw new ConfigurationParseException(PluginImplementationElement, $"The assembly '{PluginImplementationElement.Assembly.Alias}' used by plugin is disabled while the plugin is enabled. Either disable plugin or enable the assembly.", this);
        }

        #endregion
    }
}