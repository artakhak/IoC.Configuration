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

using System.Linq;
using System.Text;

namespace IoC.Configuration.ConfigurationFile
{
    public class PluginAssemblyTypeUsageValidator : IPluginAssemblyTypeUsageValidator
    {
        #region IPluginAssemblyTypeUsageValidator Interface Implementation

        public void Validate(IConfigurationFileElement requestingConfigurationFileElement, ITypeInfo typeInfo)
        {
            var uniquePluginTypes = typeInfo.GetUniquePluginTypes();

            if (uniquePluginTypes.Count == 0)
                return;

            if (uniquePluginTypes.Count > 1)
            {
                var errorMessage = new StringBuilder();

                errorMessage.Append($"Type '{typeInfo.TypeCSharpFullName}' uses types '{uniquePluginTypes[0].TypeCSharpFullName}' and '{uniquePluginTypes[1].TypeCSharpFullName}'");
                errorMessage.Append($" which are defined in assemblies '{uniquePluginTypes[0].Assembly}' and '{uniquePluginTypes[1].Assembly}' that belong to different plugins '{uniquePluginTypes[0].Assembly.Plugin.Name}' and '{uniquePluginTypes[1].Assembly.Plugin.Name}'.");
                errorMessage.Append(" Generic types cannot use types from different plugins.");

                throw new ConfigurationParseException(requestingConfigurationFileElement, errorMessage.ToString());
            }


#pragma warning disable CS0612, CS0618
            // HUCK:  Temporary handling 
            // No need to spend time to make changes to deprecated classes. 
            // Will just ignore those for now. These classes will be deleted in few months anyway.
            if (requestingConfigurationFileElement is ITypeFactory ||
                requestingConfigurationFileElement is ITypeFactoryReturnedType)
                return;

#pragma warning restore CS0612, CS0618


            // If we got here, we have only one plugin type.

            var pluginTypeInfo = uniquePluginTypes[0];
            var pluginName = pluginTypeInfo.Assembly.Plugin.Name;

            var pluginSetupElement = requestingConfigurationFileElement.GetPluginSetupElement();

            if (pluginSetupElement == null)
            {
                // Check the parents, until we find an element which allows the plugin type. 
                var currElement = requestingConfigurationFileElement;

                var pluginToLookFor = uniquePluginTypes[0].Assembly.Plugin;

                while (!(currElement is IConfiguration))
                {
                    if (currElement is ITypedItem typedItem &&
                        (currElement.Parent is ICanHaveChildElementsThatUsePluginTypeInNonPluginSection ||
                         currElement.Parent is ValueInitializerElementDecorator parentValueInitializerElementDecorator &&
                         parentValueInitializerElementDecorator.DecoratedValueInitializerElement is ICanHaveChildElementsThatUsePluginTypeInNonPluginSection))
                    {
                        if (currElement == requestingConfigurationFileElement)
                        {
                            // If requestingConfigurationFileElement is implementation element (or parameter serializer element, or collection item),
                            // it can use any plugin type.
                            return;
                        }

                        var parentUniquePluginTypes = typedItem.ValueTypeInfo.GetUniquePluginTypes();
                        if (parentUniquePluginTypes.FirstOrDefault(x => x.Assembly.Plugin == pluginToLookFor) != null)
                        {
                            // If we found a parent service implemenation or collection item, that uses the same plugin, we are fine.
                            return;
                        }
                    }

                    currElement = currElement.Parent;
                }

                // If we got here, it means the type cannot be used in this context
                var errorMessage = new StringBuilder();

                errorMessage.Append($"Type '{typeInfo.TypeCSharpFullName}'");

                if (typeInfo.GenericTypeParameters.Count > 0)
                    errorMessage.Append($" uses type '{pluginTypeInfo.TypeCSharpFullName}' which");

                errorMessage.AppendLine($" is defined in assembly '{pluginTypeInfo.Assembly.Alias}' that belongs to plugin '{pluginTypeInfo.Assembly.Plugin.Name}'.");
                errorMessage.AppendLine("Types that belong to plugins cannot be used in this context.");
                errorMessage.AppendLine($"Types that belong to plugins can be used either in elements immediately under some specific elements, such as {ConfigurationFileElementNames.Service}, {ConfigurationFileElementNames.ParameterSerializers}, or {ConfigurationFileElementNames.Collection}, or ");
                errorMessage.AppendLine($"these types can be used anywhere else in a tree below elements that use the same plugin type, and which are immediately under these elements (i.e., elements {ConfigurationFileElementNames.Service}, {ConfigurationFileElementNames.ParameterSerializers}, etc.).");
                errorMessage.AppendLine("See the documentation at https://iocconfiguration.readthedocs.io/en/latest/xml-configuration-file/plugins.html for more details.");
                throw new ConfigurationParseException(requestingConfigurationFileElement, errorMessage.ToString());
            }

            if (pluginTypeInfo.Assembly.Plugin != pluginSetupElement.Plugin)
            {
                var errorMessage = new StringBuilder();

                errorMessage.Append($"Type '{typeInfo.TypeCSharpFullName}'");

                if (typeInfo.GenericTypeParameters.Count > 0)
                    errorMessage.Append($" uses type '{pluginTypeInfo.TypeCSharpFullName}' which");

                errorMessage.AppendLine($" is defined in assembly '{pluginTypeInfo.Assembly.Alias}' that does not belong plugin '{pluginSetupElement.Plugin.Name}'.");
                errorMessage.Append($"Assembly '{pluginTypeInfo.Assembly.Alias}' belongs to plugin '{pluginTypeInfo.Assembly.Plugin.Name}'.");

                throw new ConfigurationParseException(requestingConfigurationFileElement, errorMessage.ToString());
            }
        }

        #endregion
    }
}