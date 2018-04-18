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
using System.Text;
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

        public string GenerateElementError(string message, IConfigurationFileElement parentElement = null)
        {
            var message2 = new StringBuilder();

            message2.AppendLine($"Error in element '{(parentElement != null ? parentElement : this).ElementName}':");

            message2.Append(message);
            message2.AppendLine();

            message2.Append(GenerateElementInTreeDetails());

            return message2.ToString();
        }

        public string GetAttributeValue(string attributeName)
        {
            return _xmlElement.GetAttribute(attributeName);
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

        public virtual bool Enabled => _enabled && (Parent?.Enabled ?? true);

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

        private string GenerateElementInTreeDetails()
        {
            var structure = new StringBuilder();

            structure.AppendLine("Element location in configuration file:");
            var xmlElements = new LinkedList<IConfigurationFileElement>();

            var parentElement = Parent;

            while (parentElement != null)
            {
                xmlElements.AddFirst(parentElement);
                parentElement = parentElement.Parent;
            }

            var level = 0;

            // Add details about parents
            foreach (var parentXmlElement in xmlElements)
            {
                structure.Append(new string('\t', level));
                structure.AppendLine(parentXmlElement.ToString());
                ++level;
            }

            //Add some details about siblings 
            var siblingElements = Parent.Children;
            var indentation = new string('\t', level);
            var indexInParent = -1;

            for (var i = 0; i < siblingElements.Count; ++i)
            {
                var siblingElement = siblingElements[i];

                if (siblingElement == this)
                {
                    // Add details about current element
                    //actionPrintCurrentElement();

                    indexInParent = i;
                    break;
                }

                if (i < 2 || i == siblingElements.Count - 1 || siblingElements[i + 1] == this)
                    structure.AppendLine($"{indentation}{siblingElement.XmlElementToString()}");
                else if (i == 2)
                    structure.AppendLine($"{indentation}...");
            }

            if (indexInParent < 0)
                indexInParent = siblingElements.Count;

            // Add details about current element
            structure.Append($"{indentation}{XmlElementToString()}");
            structure.Append($" <--- Element '{ElementName}' is the {indexInParent + 1}-th child element of element '{Parent.ElementName}'.");
            structure.AppendLine();

            return structure.ToString();
        }

        public override string ToString()
        {
            return XmlElementToString();
        }

        public IEnumerable<XmlElementWarning> Warnings => _warnings;

        #endregion
    }
}