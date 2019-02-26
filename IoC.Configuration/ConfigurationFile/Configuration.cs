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

using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class Configuration : ConfigurationFileElementAbstr, IConfiguration
    {
        #region  Constructors

        public Configuration([NotNull] XmlElement xmlElement) : base(xmlElement, null)
        {
        }

        #endregion

        #region IConfiguration Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IApplicationDataDirectory)
                ApplicationDataDirectory = (IApplicationDataDirectory) child;
            else if (child is IPlugins)
                Plugins = (IPlugins) child;
            else if (child is IAdditionalAssemblyProbingPaths)
                AdditionalAssemblyProbingPaths = (IAdditionalAssemblyProbingPaths) child;
            else if (child is IAssemblies)
                Assemblies = (IAssemblies) child;
            else if (child is ITypeDefinitionsElement)
                TypeDefinitions = (ITypeDefinitionsElement) child;
            else if (child is IParameterSerializers)
                ParameterSerializers = (IParameterSerializers) child;
            else if (child is IDiManagersElement)
                DiManagers = (IDiManagersElement) child;
            else if (child is ISettingsElement)
                SettingsElement = (ISettingsElement) child;
            else if (child is IWebApi)
                WebApi = (IWebApi) child;
            else if (child is IDependencyInjection)
                DependencyInjection = (IDependencyInjection) child;
            else if (child is ISettingsRequestorImplementationElement)
                SettingsRequestor = (ISettingsRequestorImplementationElement) child;
            else if (child is IStartupActionsElement)
                StartupActions = (IStartupActionsElement) child;
            else if (child is IPluginsSetup)
                PluginsSetup = (IPluginsSetup) child;
        }

        public IAdditionalAssemblyProbingPaths AdditionalAssemblyProbingPaths { get; private set; }
        public IApplicationDataDirectory ApplicationDataDirectory { get; private set; }
        public IAssemblies Assemblies { get; private set; }
        public IDependencyInjection DependencyInjection { get; private set; }
        public IDiManagersElement DiManagers { get; private set; }
        public IParameterSerializers ParameterSerializers { get; private set; }
        public IPlugins Plugins { get; private set; }
        public IPluginsSetup PluginsSetup { get; private set; }

        public void ProcessTree(ProcessConfigurationFileElement processConfigurationFileElement)
        {
            this.ProcessConfigurationFileElementAndChildren(processConfigurationFileElement);
        }

        public ISettingsElement SettingsElement { get; private set; }
        public ISettingsRequestorImplementationElement SettingsRequestor { get; private set; }
        public IStartupActionsElement StartupActions { get; private set; }
        public ITypeDefinitionsElement TypeDefinitions { get; private set; }
        public IWebApi WebApi { get; private set; }

        #endregion
    }
}