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
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class ConfigurationFileElementAbstr : IConfigurationFileElement
    {
        #region Member Variables

        private readonly List<IConfigurationFileElement> _children = new List<IConfigurationFileElement>();

        protected readonly IConfiguration _configuration;
        private bool _enabled = true;
        private IPluginSetup _pluginSetup;

        private bool _pluginSetupWasSearched;

        private readonly List<XmlElementWarning> _warnings = new List<XmlElementWarning>();

        protected readonly XmlElement _xmlElement;

        #endregion

        #region  Constructors

        public ConfigurationFileElementAbstr([NotNull] XmlElement xmlElement, IConfigurationFileElement parent)
        {
            _xmlElement = xmlElement;
            Parent = parent;

            if (parent == null)
                _configuration = (IConfiguration) this;
            else
                _configuration = parent.Configuration;
        }

        #endregion

        #region IConfigurationFileElement Interface Implementation

        public IReadOnlyList<IConfigurationFileElement> Children => _children;

        public IConfiguration Configuration => _configuration;

        public string ElementName => _xmlElement.Name;

        string IConfigurationFileElement.GenerateElementError(string message, IConfigurationFileElement parentElement)
        {
            return ErrorHelperAmbientContext.Context.GenerateElementError(this, message, parentElement);
        }

        public string GetAttributeValue(string attributeName)
        {
            var attributeValue = _xmlElement.GetAttribute(attributeName)?.Trim();

            if (string.IsNullOrEmpty(attributeValue))
                throw new ConfigurationParseException(this,
                    $"Attribute '{attributeName}' should have valid non-empty value.");

            return attributeValue;
        }

        [CanBeNull]
        public IPluginSetup GetParentPluginSetupElement()
        {
            if (this is IConfiguration)
                return null;

            var parent = Parent;

            while (!(parent is IConfiguration))
            {
                if (parent is IPluginSetup pluginSetup)
                    return pluginSetup;

                parent = parent.Parent;
            }

            return null;
        }


        public IPluginSetup GetPluginSetupElement()
        {
            if (_pluginSetup != null)
                return _pluginSetup;

            if (_pluginSetupWasSearched)
                return null;

            _pluginSetupWasSearched = true;

            if (this is IConfiguration)
                return null;

            if (this is IPluginSetup pluginSetup)
                _pluginSetup = pluginSetup;
            else
                _pluginSetup = Parent.GetPluginSetupElement();

            return _pluginSetup;
        }

        public bool HasAttribute(string name)
        {
            return _xmlElement.HasAttribute(name);
        }

        public IConfigurationFileElement Parent { get; }

        public string XmlElementToString()
        {
            return _xmlElement.XmlElementToString();
        }

        #endregion

        #region Current Type Interface

        public virtual void AddChild(IConfigurationFileElement child)
        {
            _children.Add(child);
        }

        public virtual void BeforeChildInitialize(IConfigurationFileElement child)
        {
        }

        public virtual bool Enabled => _enabled && (OwningPluginElement?.Enabled ?? true) && (Parent?.Enabled ?? true);

        public virtual void Initialize()
        {
            if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.Enabled))
                _enabled = this.GetEnabledAttributeValue();
        }

        public virtual IPluginElement OwningPluginElement => Parent?.OwningPluginElement;

        public virtual void ValidateAfterChildrenAdded()
        {
        }

        public virtual void ValidateOnTreeConstructed()
        {
        }

        #endregion

        #region Member Functions

        public override string ToString()
        {
            return XmlElementToString();
        }

        public IEnumerable<XmlElementWarning> Warnings => _warnings;

        #endregion
    }
}