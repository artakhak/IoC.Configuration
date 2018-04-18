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
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class DiManagersElement : ConfigurationFileElementAbstr, IDiManagersElement
    {
        #region Member Variables

        [NotNull]
        private string _activeDiManagerName = string.Empty;

        [NotNull]
        private readonly Dictionary<string, IDiManagerElement> _diManagerNameToDiManagerMap = new Dictionary<string, IDiManagerElement>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region  Constructors

        public DiManagersElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IDiManagersElement Interface Implementation

        public IDiManagerElement ActiveDiManagerElement { get; private set; }

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IDiManagerElement)
            {
                var diManagerElement = (IDiManagerElement) child;

                if (_diManagerNameToDiManagerMap.ContainsKey(diManagerElement.Name))
                    throw new ConfigurationParseException(diManagerElement, $"Multiple occurrences of '{ConfigurationFileElementNames.DiManager}' elements with the same value for attribute '{ConfigurationFileAttributeNames.Name}'. The value of attibute is '{diManagerElement.Name}'.", this);

                _diManagerNameToDiManagerMap[diManagerElement.Name] = diManagerElement;
                if (_activeDiManagerName.Equals(diManagerElement.Name, StringComparison.OrdinalIgnoreCase))
                    ActiveDiManagerElement = diManagerElement;
            }
        }

        public IEnumerable<IDiManagerElement> AllDiManagers => _diManagerNameToDiManagerMap.Values;

        public override void Initialize()
        {
            base.Initialize();
            _activeDiManagerName = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.ActiveDiManagerName);
        }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            if (ActiveDiManagerElement == null)
                throw new ConfigurationParseException(this, $"No dependency injection manager named '{_activeDiManagerName}' was found.");
        }

        #endregion
    }
}