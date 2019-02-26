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

using System.IO;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class WebApiControllerAssembly : ConfigurationFileElementAbstr, IWebApiControllerAssembly
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        [CanBeNull]
        private System.Reflection.Assembly _loadedAssembly;

        #endregion

        #region  Constructors

        public WebApiControllerAssembly([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                        [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region IWebApiControllerAssembly Interface Implementation

        public IAssembly Assembly { get; private set; }

        /// <summary>
        ///     Gets the loaded assembly. The value is non-null only if assembly exists and is enabled.
        ///     If assembly does not exist, an error will be log.
        ///     Assembly might be disabled if the assembly belongs to a plugin which is disabled.
        /// </summary>
        public System.Reflection.Assembly LoadedAssembly => Enabled ? _loadedAssembly : null;

        #endregion

        #region Member Functions

        public override void Initialize()
        {
            base.Initialize();

            if (Enabled)
            {
                Assembly = Helpers.GetAssemblySettingByAssemblyAlias(this, this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Assembly));

                if (OwningPluginElement == null)
                {
                    if (Assembly.OwningPluginElement != null)
                        throw new ConfigurationParseException(this, $"Assembly  '{Assembly.Name}' belongs to plugin '{Assembly.OwningPluginElement.Name}' and can be only used in '{ElementName}' element in plugin '{Assembly.OwningPluginElement.Name}'.");
                }
                else if (OwningPluginElement != Assembly.OwningPluginElement)
                {
                    throw new ConfigurationParseException(this, $"Assembly  '{Assembly.Name}' does not belong to plugin '{OwningPluginElement.Name}'. Only assemblies of plugin  '{OwningPluginElement.Name}' can be used.");
                }

                try
                {
                    _loadedAssembly = _assemblyLocator.LoadAssembly(Path.GetFileName(Assembly.AbsolutePath), Path.GetDirectoryName(Assembly.AbsolutePath));
                }
                catch
                {
                    throw new ConfigurationParseException(this, $"Failed to load assembly '{Assembly.AbsolutePath}'.");
                }
            }
        }

        #endregion
    }
}