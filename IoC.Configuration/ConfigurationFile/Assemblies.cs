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
    public class Assemblies : ConfigurationFileElementAbstr, IAssemblies
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<string, IAssembly> _aliasToAssemblyMap = new Dictionary<string, IAssembly>(StringComparer.OrdinalIgnoreCase);

        [NotNull]
        private readonly Dictionary<string, IAssembly> _nameToAssemblyMap = new Dictionary<string, IAssembly>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region  Constructors

        public Assemblies([NotNull] XmlElement xmlElement, [CanBeNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IAssemblies Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            if (child is IAssembly)
            {
                var assembly = (IAssembly) child;

                if (_nameToAssemblyMap.ContainsKey(assembly.Name))
                    throw new ConfigurationParseException(child, $"Assembly with name '{assembly.Name}' appears multiple times.", this);

                if (_aliasToAssemblyMap.ContainsKey(assembly.Alias))
                    throw new ConfigurationParseException(child, $"Assembly with alias '{assembly.Alias}' appears multiple times.", this);

                _nameToAssemblyMap[assembly.Name] = assembly;
                _aliasToAssemblyMap[assembly.Alias] = assembly;
            }

            base.AddChild(child);
        }

        public IEnumerable<IAssembly> AllAssemblies => _nameToAssemblyMap.Values;

        public IAssembly GetAssemblyByAlias(string alias)
        {
            return _aliasToAssemblyMap.TryGetValue(alias, out var assembly) ? assembly : null;
        }

        #endregion
    }
}