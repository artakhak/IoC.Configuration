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
    public class TypeDefinitionsElement : ConfigurationFileElementAbstr, ITypeDefinitionsElement, ICanHaveChildElementsThatUsePluginTypeInNonPluginSection
    {
        #region Member Variables

        private readonly Dictionary<string, INamedTypeDefinitionElement> _aliasToTypeDefinition = new Dictionary<string, INamedTypeDefinitionElement>(StringComparer.OrdinalIgnoreCase);
        private readonly List<INamedTypeDefinitionElement> _typeDefinitions = new List<INamedTypeDefinitionElement>();

        #endregion

        #region  Constructors

        public TypeDefinitionsElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region ITypeDefinitionsElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);
            if (child is INamedTypeDefinitionElement typeDefinitionElement)
            {
                if (_aliasToTypeDefinition.ContainsKey(typeDefinitionElement.Alias))
                    throw new ConfigurationParseException(this, $"There is already a type definition with '{ConfigurationFileAttributeNames.Alias}' of '{typeDefinitionElement.Alias}'.");

                _aliasToTypeDefinition[typeDefinitionElement.Alias] = typeDefinitionElement;
                _typeDefinitions.Add(typeDefinitionElement);
            }
        }

        public IReadOnlyList<INamedTypeDefinitionElement> AllTypeDefinitions => _typeDefinitions;

        public INamedTypeDefinitionElement GetTypeDefinition(string alias)
        {
            if (_aliasToTypeDefinition.TryGetValue(alias, out var typeDefinitionElement))
                return typeDefinitionElement;

            if (GetPluginSetupElement() != null)
            {
                // Didn't find in plugin. Try to find the type definition in global type definitions.
                return Configuration.TypeDefinitions.GetTypeDefinition(alias);
            }

            return null;
        }

        #endregion
    }
}