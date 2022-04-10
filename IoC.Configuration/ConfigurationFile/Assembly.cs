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
using System.IO;
using System.Text;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    public class Assembly : ConfigurationFileElementAbstr, IAssembly
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        [NotNull]
        private readonly IIdentifierValidator _identifierValidator;

        #endregion

        #region  Constructors

        public Assembly([NotNull] XmlElement xmlElement, IConfigurationFileElement parent, [NotNull] IAssemblyLocator assemblyLocator,
                        [NotNull] IIdentifierValidator identifierValidator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
            _identifierValidator = identifierValidator;
        }

        #endregion

        #region IAssembly Interface Implementation

        public string AbsolutePath { get; private set; }

        public string Alias { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            Name = this.GetNameAttributeValue();

            //if (!Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            //    throw new ConfigurationParseException(this, $"The value of '{ConfigurationFileAttributeNames.Name}' should be a file name with extension '.dll'.", false);

            if (Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                throw new ConfigurationParseException(this, $"The value of '{ConfigurationFileAttributeNames.Name}' should be an assembly file name without the file extension'.");

            Alias = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Alias);

            _identifierValidator.Validate(this, ConfigurationFileAttributeNames.Alias, Alias);

            if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.Plugin))
            {
                var pluginName = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Plugin);

                var plugin = _configuration.Plugins?.GetPlugin(pluginName);

                if (plugin == null)
                    throw new ConfigurationParseException(this, $"There is no plugin with name '{pluginName}'.");

                Plugin = plugin;
            }

            string assemblyPath;

            if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.OverrideDirectory))
            {
                assemblyPath = Path.Combine(this.GetAttributeValue<string>(ConfigurationFileAttributeNames.OverrideDirectory), $"{Name}.dll");

                if (!File.Exists(assemblyPath))
                    throw new ConfigurationParseException(this, $"Could not find assembly '{Name}'.");
            }
            else
            {
                assemblyPath = _assemblyLocator.FindAssemblyPath(Name, Plugin?.Name, out var searchedDirectories);

                if (string.IsNullOrWhiteSpace(assemblyPath))
                {
                    var errorMessage = new StringBuilder();
                    errorMessage.AppendLine($"Could not find assembly '{Name}'. The following directories were searched:");

                    foreach (var searchedDiretory in searchedDirectories)
                        errorMessage.AppendLine($"  '{searchedDiretory}'");

                    throw new ConfigurationParseException(this, errorMessage.ToString());
                }
            }

            AbsolutePath = assemblyPath;

            LogHelper.Context.Log.InfoFormat("Resolved assembly '{0}' as '{1}'", Name, assemblyPath);

            if (Plugin != null)
            {
                var assemblyDirectory = Path.GetDirectoryName(assemblyPath);

                if (string.Compare(assemblyDirectory, Plugin.GetPluginDirectory(), StringComparison.OrdinalIgnoreCase) != 0)
                    throw new ConfigurationParseException(this, $"The assembly '{Name}' is configured as a plugin assembly for plugin '{Plugin.Name}'. Therefore the resolved assembly file should be in plugin directory '{Plugin.GetPluginDirectory()}'. Either remove the '{ConfigurationFileAttributeNames.Plugin}' attribute, or make sure that the assembly is in plugin directory and no other assembly with the same name exists in other probing folders.");
            }

            //if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.LoadAssemblyAlways) &&
            //    this.GetAttributeValue<bool>(ConfigurationFileAttributeNames.LoadAssemblyAlways))
            //{
            //    try
            //    {
            //        LogHelper.Context.Log.InfoFormat("The value of attribute '{0}' is true. Loading assembly {1}.", ConfigurationFileAttributeNames.LoadAssemblyAlways, assemblyPath);
            //        _assemblyLocator.LoadAssembly(Path.GetFileName(assemblyPath), Path.GetDirectoryName(assemblyPath));
            //    }
            //    catch (Exception e)
            //    {
            //        LogHelper.Context.Log.Warn(e.Message, e);
            //        throw new ConfigurationParseException(this, $"Failed to load the assembly '{assemblyPath}'.");
            //    }
            //}
        }

        public string Name { get; private set; }

        public override IPluginElement OwningPluginElement => Plugin;
        public IPluginElement Plugin { get; private set; }

        #endregion

        #region Member Functions

        public override string ToString()
        {
            return $"Assembly {{{ConfigurationFileAttributeNames.Name}: '{Name}', {ConfigurationFileAttributeNames.Alias}: '{Alias}'}}";
        }

        #endregion
    }
}