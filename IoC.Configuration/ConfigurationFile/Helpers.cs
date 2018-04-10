using System;
using System.IO;
using System.Text;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    public static class Helpers
    {
        #region Member Functions

        /// <summary>
        ///     Converts <paramref name="configurationFileElement" /> to type <typeparamref name="T" />. Throws an exception if the
        ///     conversion fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurationFileElement"></param>
        /// <returns>Converted value.</returns>
        /// <exception cref="ConfigurationParseException">Throws this exception.</exception>
        [NotNull]
        public static T ConvertTo<T>(IConfigurationFileElement configurationFileElement) where T : class, IConfigurationFileElement
        {
            var convertedValue = configurationFileElement as T;

            if (convertedValue == null)
                throw new ConfigurationParseException(configurationFileElement, $"Could not convert to {typeof(T).FullName}.");

            return convertedValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="requestorFileElement"></param>
        /// <param name="assemblyAlias"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationParseException">
        ///     Throws this exception if the assembly with specified alias
        ///     does not exitst in configuration file.
        /// </exception>
        internal static IAssembly GetAssemblySettingByAssemblyAlias([NotNull] IConfigurationFileElement requestorFileElement, [NotNull] string assemblyAlias)
        {
            var assemblySetting = requestorFileElement.Configuration.Assemblies.GetAssemblyByAlias(assemblyAlias);

            if (assemblySetting == null)
                throw new ConfigurationParseException(requestorFileElement, $"No assembly with alias '{assemblyAlias}' is declared in '{ConfigurationFileElementNames.Assemblies}' element.");

            return assemblySetting;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurationFileElement"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationParseException">
        ///     Throws this exception if the attribute value cannot be converted to type
        ///     of 'T'.
        /// </exception>
        internal static T GetAttributeValue<T>(this IConfigurationFileElement configurationFileElement, string attributeName)
        {
            var typeOfConvertedValue = typeof(T);

            var attributeValue = configurationFileElement.GetAttributeValue(attributeName)?.Trim();

            try
            {
                if (!string.IsNullOrEmpty(attributeValue))
                {
                    if (typeOfConvertedValue == typeof(double) || typeOfConvertedValue == typeof(float))
                        return (T) (object) double.Parse(attributeValue);

                    if (typeOfConvertedValue == typeof(long))
                        return (T) (object) long.Parse(attributeValue);

                    if (typeOfConvertedValue == typeof(int))
                        return (T) (object) int.Parse(attributeValue);

                    if (typeOfConvertedValue == typeof(short))
                        return (T) (object) short.Parse(attributeValue);

                    if (typeOfConvertedValue == typeof(byte))
                        return (T) (object) short.Parse(attributeValue);

                    if (typeOfConvertedValue == typeof(bool))
                        return (T) (object) bool.Parse(attributeValue);

                    if (typeOfConvertedValue == typeof(DateTime))
                        return (T) (object) DateTime.Parse(attributeValue);

                    if (typeOfConvertedValue == typeof(string))
                        return (T) (object) attributeValue;

                    LogHelper.Context.Log.WarnFormat("Unhandled type '{0}' in '{1}.{2}().'", typeOfConvertedValue, typeof(Helpers).FullName, nameof(GetAttributeValue));
                }
            }
            catch
            {
                // We will throw exception next.
            }

            // If we didn't return anything, throw.
            throw new ConfigurationParseException(configurationFileElement,
                $"Attribute '{attributeName}' should have valid non-empty value of type '{typeof(T).FullName}'.");
        }

        /// <summary>
        /// </summary>
        /// <param name="configurationFileElement"></param>
        /// <exception cref="ConfigurationParseException">
        ///     Throws this exception if the attribute value cannot be conveted to a
        ///     boolean value.
        /// </exception>
        internal static bool GetEnabledAttributeValue(this IConfigurationFileElement configurationFileElement)
        {
            return configurationFileElement.GetAttributeValue<bool>(ConfigurationFileAttributeNames.Enabled);
        }

        /// <summary>
        /// </summary>
        /// <param name="configurationFileElement"></param>
        /// <exception cref="ConfigurationParseException">Throws this exception if the attribute value is null or empty string.</exception>
        internal static string GetNameAttributeValue(this IConfigurationFileElement configurationFileElement)
        {
            return configurationFileElement.GetAttributeValue<string>(ConfigurationFileAttributeNames.Name);
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyLocator"></param>
        /// <param name="requestorFileElement"></param>
        /// <param name="assemblyElement"></param>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationParseException">Throws this exception if the type is not in an assembly.</exception>
        internal static Type GetTypeInAssembly([NotNull] IAssemblyLocator assemblyLocator, [NotNull] IConfigurationFileElement requestorFileElement, [NotNull] IAssembly assemblyElement, [NotNull] string typeFullName)
        {
            System.Reflection.Assembly assembly = null;

            try
            {
                assembly = assemblyLocator.LoadAssembly(Path.GetFileName(assemblyElement.AbsolutePath), Path.GetDirectoryName(assemblyElement.AbsolutePath));
            }
            catch
            {
                throw new ConfigurationParseException(assemblyElement, $"Failed to load assembly '{assemblyElement.AbsolutePath}'.");
            }

            var type = assembly.GetType(typeFullName, false, false);

            if (type == null)
                throw new ConfigurationParseException(requestorFileElement, $"Type '{typeFullName}' was not found in an assembly '{assemblyElement.AbsolutePath}'. Assembly is specified in an element '{ConfigurationFileElementNames.Assembly}' with the value of attribute '{ConfigurationFileAttributeNames.Alias}' equal to '{assemblyElement.Alias}'.");

            return type;
        }

        internal static void ValidateIdentifier(this IConfigurationFileElement configurationFileElement, string attributeName)
        {
            var attributeValue = configurationFileElement.GetAttributeValue<string>(attributeName);

            var isValidIdentifier = attributeValue.Length > 0;

            // TODO: Replace with RegEx or 
            for (var i = 0; i < attributeValue.Length; ++i)
            {
                var currChar = attributeValue[i];

                if (currChar == '_')
                    continue;

                if (char.IsNumber(currChar))
                {
                    if (i == 0)
                    {
                        isValidIdentifier = false;
                        break;
                    }
                }
                else if (!(currChar >= 'a' && currChar <= 'z' || currChar >= 'A' && currChar <= 'Z'))
                {
                    isValidIdentifier = false;
                    break;
                }
            }

            if (!isValidIdentifier)
                throw new ConfigurationParseException(configurationFileElement,
                    $"The value of attribute '{attributeName}' is not a valid identifier. Valid identifier chould contain only alphanumeric characters or underscore '_' and should not start with a number.");
        }

        public static string XmlElementToString(this XmlElement xmlElement)
        {
            var xmlElementText = new StringBuilder();

            // Add details about current element
            xmlElementText.Append($"<{xmlElement.Name}");

            for (var attrIndex = 0; attrIndex < xmlElement.Attributes.Count; ++attrIndex)
            {
                var attriute = xmlElement.Attributes[attrIndex];
                xmlElementText.Append($" {attriute.Name}=\"{attriute.Value}\"");
            }

            if (!xmlElement.HasChildNodes)
                xmlElementText.Append("/");

            xmlElementText.Append(">");

            return xmlElementText.ToString();
        }

        #endregion
    }
}