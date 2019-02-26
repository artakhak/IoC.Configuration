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

using System.Text;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class DisabledPluginTypeUsedConfigurationParseException : ConfigurationParseException
    {
        #region  Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DisabledPluginTypeUsedConfigurationParseException" /> class.
        /// </summary>
        /// <param name="configurationFileElement">The configuration file element.</param>
        /// <param name="typeInfoInfo">
        ///     <see cref="ITypeInfo" /> for a type in disabled plugin, or which contains a disabled plugin
        ///     type, if the type is a generic type.
        /// </param>
        /// <param name="disabledPluginTypeInfo">The disabled plugin type information.</param>
        /// <param name="parentElement">The parent element.</param>
        public DisabledPluginTypeUsedConfigurationParseException([NotNull] IConfigurationFileElement configurationFileElement,
                                                                 [NotNull] ITypeInfo typeInfoInfo,
                                                                 [NotNull] ITypeInfo disabledPluginTypeInfo,
                                                                 IConfigurationFileElement parentElement = null) :
            base(configurationFileElement, GenerateErrorMessage(typeInfoInfo, disabledPluginTypeInfo), parentElement, true)
        {
        }

        #endregion

        #region Member Functions

        private static string GenerateErrorMessage([NotNull] ITypeInfo typeInfoInfo, [NotNull] ITypeInfo disabledPluginTypeInfo)
        {
            var errorString = new StringBuilder();
            errorString.Append($"The type '{typeInfoInfo.TypeCSharpFullName}'");

            if (typeInfoInfo.GenericTypeParameters.Count > 0)
                errorString.Append($" uses type '{disabledPluginTypeInfo.TypeCSharpFullName}' which");

            errorString.AppendLine($" is defined in assembly '{disabledPluginTypeInfo.Assembly.Alias}' that belongs to disabled plugin '{disabledPluginTypeInfo.Assembly.Plugin.Name}'.");
            errorString.AppendLine($"Either enable the plugin '{disabledPluginTypeInfo.Assembly.Plugin}', or get rid of usage of this type.");

            return errorString.ToString();
        }

        #endregion
    }
}