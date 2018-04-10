using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class AdditionalAssemblyProbingPaths : ConfigurationFileElementAbstr, IAdditionalAssemblyProbingPaths
    {
        #region Member Variables

        [NotNull]
        private readonly LinkedList<IProbingPath> _probingPaths = new LinkedList<IProbingPath>();

        #endregion

        #region  Constructors

        public AdditionalAssemblyProbingPaths([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IAdditionalAssemblyProbingPaths Interface Implementation

        public IEnumerable<IProbingPath> ProbingPaths => _probingPaths;

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            var probingPathsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var child in Children)
                if (child is IProbingPath)
                {
                    var probingPath = (IProbingPath) child;

                    if (probingPathsSet.Contains(probingPath.Path))
                        throw new ConfigurationParseException(child, $"Multiple occurrences of element '{ConfigurationFileElementNames.ProbingPath}', with the same value of '{ConfigurationFileAttributeNames.Path}' attribute.", this);

                    _probingPaths.AddLast(probingPath);

                    probingPathsSet.Add(probingPath.Path);

                    if (_configuration.Plugins != null)
                        foreach (var plugin in _configuration.Plugins.AllPlugins)
                            if (string.Compare(plugin.GetPluginDirectory(), probingPath.Path, StringComparison.OrdinalIgnoreCase) == 0)
                                throw new ConfigurationParseException(child, $"Directory '{probingPath}' is used both in both '{ConfigurationFileElementNames.Plugin}' and '{ConfigurationFileElementNames.ProbingPath}' elements.", this);
                }
        }

        #endregion
    }
}