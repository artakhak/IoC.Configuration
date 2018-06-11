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
using System.Text;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration
{
    public static class MessagesHelper
    {
        #region Member Functions

        /// <summary>
        ///     Returns a string: The value of <paramref name="registerIfNotRegisteredSubjectName" /> can be true only if there is
        ///     a single implementation for the service.
        /// </summary>
        /// <param name="registerIfNotRegisteredSubjectName"></param>
        [NotNull]
        public static string GetMultipleImplementationsWithRegisterIfNotRegisteredOptionMessage([NotNull] string registerIfNotRegisteredSubjectName)
        {
            return $"The value of {registerIfNotRegisteredSubjectName} can be true only if there is a single implementation for the service.";
        }

        [NotNull]
        public static string GetNoImplementationsForServiceMessage([NotNull] Type serviceType)
        {
            return $"No implementation is provided for service '{serviceType.FullName}'.";
        }

        [NotNull]
        public static string GetServiceImplmenentationTypeAssemblyBelongsToPluginMessage([NotNull] Type implementationType, [NotNull] string assemblyAlias, [NotNull] string pluginName)
        {
            return $"The settings requestor type '{implementationType.FullName}' is defined in assembly '{assemblyAlias}' which belongs to plugin '{pluginName}'. The assembly where the type is defined should not be associated with any plugin.";
        }

        
        public static void LogElementDisabledWarning([NotNull]IConfigurationFileElement configurationFileElement, [CanBeNull] IAssembly assembly, bool logInfo = false)
        {
            if (configurationFileElement.Enabled)
                return;

            // Lets see if this element is a child of PluginSetup element, in which case we do not want to log any warning
            // since we already logged a warning for plugin is disabled.
            if (configurationFileElement.GetParentPluginSetupElement() != null)
                return;

            var warning = new StringBuilder();
            warning.AppendLine($"Element '{configurationFileElement.ToString()}' is disabled.");

            var pluginElement = assembly?.Plugin ?? configurationFileElement.OwningPluginElement;

            if (pluginElement != null && !pluginElement.Enabled)
                warning.AppendLine($"   To enable this element enable the plugin '{pluginElement.Name}'.");

            if (logInfo)
                LogHelper.Context.Log.Info(warning.ToString());
            else
                LogHelper.Context.Log.Warn(warning.ToString());
        }
        #endregion
    }
}