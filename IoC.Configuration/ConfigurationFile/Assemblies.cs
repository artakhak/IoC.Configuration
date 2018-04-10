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