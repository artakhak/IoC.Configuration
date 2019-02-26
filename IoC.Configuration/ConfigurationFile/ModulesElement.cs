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
    public class ModulesElement : ConfigurationFileElementAbstr, IModulesElement
    {
        #region Member Variables

        [NotNull]
        [ItemNotNull]
        private readonly LinkedList<IModuleElement> _allModuleElements = new LinkedList<IModuleElement>();

        [NotNull]
        private readonly Dictionary<Type, IModuleElement> _moduleTypeToModuleSetting = new Dictionary<Type, IModuleElement>();

        #endregion

        #region  Constructors

        public ModulesElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IModulesElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IModuleElement moduleElement && moduleElement.DiModule != null)
            {
                var moduleType = moduleElement.DiModule.GetType();

                if (_moduleTypeToModuleSetting.ContainsKey(moduleType))
                    throw new ConfigurationParseException(moduleElement, $"Multiple occurrences of dependency injection module '{moduleType.FullName}'.", this);

                _moduleTypeToModuleSetting[moduleType] = moduleElement;
                _allModuleElements.AddLast(moduleElement);
            }
        }

        public IEnumerable<IModuleElement> Modules => _allModuleElements;

        #endregion
    }
}