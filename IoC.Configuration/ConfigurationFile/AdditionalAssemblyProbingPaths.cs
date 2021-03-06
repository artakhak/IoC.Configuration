// This software is part of the IoC.Configuration library
// Copyright � 2018 IoC.Configuration Contributors
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