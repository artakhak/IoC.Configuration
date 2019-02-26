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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    public class TypeHelper : ITypeHelper
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        [NotNull]
        private readonly IPluginAssemblyTypeUsageValidator _pluginAssemblyTypeUsageValidator;

        private readonly Dictionary<string, ITypeInfo> _typeFullNameToTypeInfo = new Dictionary<string, ITypeInfo>(StringComparer.Ordinal);

        [NotNull]
        private readonly ITypeParser _typeParser;

        #endregion

        #region  Constructors

        public TypeHelper([NotNull] IAssemblyLocator assemblyLocator,
                          [NotNull] ITypeParser typeParser,
                          [NotNull] IPluginAssemblyTypeUsageValidator pluginAssemblyTypeUsageValidator)
        {
            _assemblyLocator = assemblyLocator;
            _typeParser = typeParser;
            _pluginAssemblyTypeUsageValidator = pluginAssemblyTypeUsageValidator;
        }

        #endregion

        #region ITypeHelper Interface Implementation

        public ITypeInfo GetTypeInfo(IConfigurationFileElement requestingConfigurationFileElement,
                                     string typeAttributeName, string assemblyAttributeName,
                                     string typeRefAttributeName)
        {
            return GetTypeInfo(requestingConfigurationFileElement, typeAttributeName, assemblyAttributeName, typeRefAttributeName, Enumerable.Empty<ITypeInfo>());
        }


        public ITypeInfo GetTypeInfo(IConfigurationFileElement requestingConfigurationFileElement, string typeAttributeName, string assemblyAttributeName,
                                     string typeRefAttributeName, IEnumerable<ITypeInfo> genericTypeParameters)
        {
            ITypeInfo tryGetKnownTypeInfo()
            {
                // Lets have this in a block, so that typeInfo is not accessible in the rst of the code.
                if (TryGetPrimitiveType(requestingConfigurationFileElement, out var typeInfo))
                    return typeInfo;

                if (requestingConfigurationFileElement.HasAttribute(typeRefAttributeName))
                {
                    if (requestingConfigurationFileElement.HasAttribute(typeAttributeName) || requestingConfigurationFileElement.HasAttribute(assemblyAttributeName))
                        throw new ConfigurationParseException(requestingConfigurationFileElement, $"Attributes '{typeAttributeName}' and '{assemblyAttributeName}' cannot be used to specify a type, if attribute '{typeRefAttributeName}' is used.");

                    var typeRefAttributeValue = requestingConfigurationFileElement.GetAttributeValue(typeRefAttributeName);

                    var typeDefinition = requestingConfigurationFileElement.GetTypeDefinition(typeRefAttributeValue);
                    if (typeDefinition?.ValueTypeInfo == null)
                        throw new ConfigurationParseException(requestingConfigurationFileElement, $"Invalid value of attribute '{typeRefAttributeName}'. Type definition with alias '{typeRefAttributeValue}' was not found in section '{ConfigurationFileElementNames.TypeDefinitions}'.");

                    return typeDefinition?.ValueTypeInfo;
                }

                return null;
            }

            var typeInfo2 = tryGetKnownTypeInfo();

            if (typeInfo2 != null)
            {
                _pluginAssemblyTypeUsageValidator.Validate(requestingConfigurationFileElement, typeInfo2);
                return typeInfo2;
            }

            if (!requestingConfigurationFileElement.HasAttribute(typeAttributeName))
            {
                throw new ConfigurationParseException(requestingConfigurationFileElement,
                    $"A value for either '{typeRefAttributeName}' or '{typeAttributeName}' should be provided. If attribute '{typeAttributeName}' is present, an optional attribute '{assemblyAttributeName}' value can be provided as well, to enforce that the type is in specified assembly.");
            }

            var typeAttributeValue = requestingConfigurationFileElement.GetAttributeValue<string>(typeAttributeName);

            return GetTypeInfoFromTypeFullName(requestingConfigurationFileElement, typeAttributeValue, genericTypeParameters, typeAttributeValue, assemblyAttributeName);
        }

        /// <summary>
        ///     Gets an instance of <see cref="ITypeInfo" /> by looking for the type in assemblies specified in section assemblies.
        /// </summary>
        /// <param name="requestingConfigurationFileElement">An element, where the type is specified.</param>
        /// <param name="type">Type</param>
        /// <exception cref="ConfigurationParseException">Throws this exception, if the type is not found.</exception>
        public ITypeInfo GetTypeInfoFromType(IConfigurationFileElement requestingConfigurationFileElement, Type type)
        {
            var genericTypeParameters = new List<ITypeInfo>();

            if (type.GenericTypeArguments != null)
            {
                foreach (var genericTypeParameter in type.GenericTypeArguments)
                    genericTypeParameters.Add(GetTypeInfoFromType(requestingConfigurationFileElement, genericTypeParameter));
            }

            var searchedAssemblies = requestingConfigurationFileElement.Configuration.Assemblies.AllAssembliesIncludingAssembliesNotInConfiguration;

            var assemblyName = type.Assembly.GetName().Name;
            var assembly = searchedAssemblies.FirstOrDefault(x => assemblyName.Equals(x.Name, StringComparison.OrdinalIgnoreCase));

            if (assembly == null)
            {
                var errorStrBldr = new StringBuilder();
                errorStrBldr.AppendLine($"Type '{type.FullName}' is declared in assembly '{assemblyName}' which is not defined in IoC.Configuration file.");
                errorStrBldr.AppendLine($"Make sure that the assembly '{assemblyName}' is included in element iocConfiguration/{ConfigurationFileElementNames.Assemblies}'.");
                errorStrBldr.Append(GetSearchedAssembliesText(searchedAssemblies));
            }

            ITypeInfo typeInfo;
            if (type.IsArray)
                typeInfo = TypeInfo.CreateArrayTypeInfo(type, assembly, genericTypeParameters);
            else
                typeInfo = TypeInfo.CreateNonArrayTypeInfo(type, assembly, genericTypeParameters);

            _pluginAssemblyTypeUsageValidator.Validate(requestingConfigurationFileElement, typeInfo);
            return typeInfo;
        }

        /// <summary>
        ///     Gets an instance of <see cref="ITypeInfo" /> by looking for the type in assemblies specified in section assemblies.
        /// </summary>
        /// <param name="requestingConfigurationFileElement">An element, where the type is specified.</param>
        /// <param name="typeFullName">Type full name.</param>
        /// <exception cref="ConfigurationParseException">Throws this exception, if the type is not found.</exception>
        public ITypeInfo GetTypeInfoFromTypeFullName(IConfigurationFileElement requestingConfigurationFileElement, string typeFullName)
        {
            return GetTypeInfoFromTypeFullName(requestingConfigurationFileElement, typeFullName, new ITypeInfo[0], null, null);
        }

        #endregion

        #region Member Functions

        private bool GetPrimitiveTypeInfo<T>(IConfiguration configuration, out ITypeInfo typeInfo)
        {
            var primitiveType = typeof(T);
            typeInfo = TypeInfo.CreateNonArrayTypeInfo(primitiveType, configuration.Assemblies.MsCorlibAssembly, null);
            return true;
        }

        private string GetSearchedAssembliesText(IEnumerable<IoC.Configuration.IAssembly> assembliesToSearch)
        {
            var searchedAssembliesText = new StringBuilder();
            searchedAssembliesText.AppendLine($"The following assemblies specified in element 'iocConfiguration/{ConfigurationFileElementNames.Assemblies}' were searched:");

            foreach (var currAssembly in assembliesToSearch)
                searchedAssembliesText.AppendLine($"  Assembly name: {currAssembly.Name}, assembly alias: {currAssembly.Alias}, assembly path: {currAssembly.AbsolutePath}.");

            return searchedAssembliesText.ToString();
        }

        private ITypeInfo GetTypeInfo2([NotNull] IConfigurationFileElement requestingConfigurationFileElement,
                                       [NotNull] ITypeData typeData,
                                       [NotNull] string assemblyAttributeName,
                                       [NotNull] [ItemNotNull] IReadOnlyList<ITypeInfo> genericTypeParametersList)
        {
            IAssembly assembly = null;

            if (assemblyAttributeName != null && requestingConfigurationFileElement.HasAttribute(assemblyAttributeName))
                assembly = Helpers.GetAssemblySettingByAssemblyAlias(requestingConfigurationFileElement, requestingConfigurationFileElement.GetAttributeValue<string>(assemblyAttributeName));

            string typeFullNameWithGenericParameters;

            {
                var typeFullNameWithGenericParametersStrBldr = new StringBuilder();
                if (genericTypeParametersList.Count > 0)
                    typeFullNameWithGenericParametersStrBldr.Append($"{typeData.TypeFullNameWithoutGenericParameters}<{string.Join(",", genericTypeParametersList.Select(x => x.TypeCSharpFullName))}>");
                else
                    typeFullNameWithGenericParametersStrBldr.Append(typeData.TypeFullNameWithoutGenericParameters);

                if (typeData.IsArray)
                    typeFullNameWithGenericParametersStrBldr.Append("[]");

                typeFullNameWithGenericParameters = typeFullNameWithGenericParametersStrBldr.ToString();
            }

            System.Reflection.Assembly assemblyToLoadTypeFrom = null;

            if (assembly != null)
                assemblyToLoadTypeFrom = LoadAssembly(assembly);

            if (!_typeFullNameToTypeInfo.TryGetValue(typeFullNameWithGenericParameters, out var typeInfo))
            {
                IEnumerable<IoC.Configuration.IAssembly> assembliesToSearch;

                if (assembly != null)
                {
                    assembliesToSearch = new IoC.Configuration.IAssembly[] {assembly};
                }
                else
                {
                    assembliesToSearch = requestingConfigurationFileElement.Configuration.Assemblies.AllAssembliesIncludingAssembliesNotInConfiguration;
                }

                ITypeInfo tryFindTypeInAssemblies(bool tryLocalType)
                {
                    var internalTypeNameStrBldr = new StringBuilder();

                    if (tryLocalType)
                    {
                        var typeFullNameWithoutGenericParameters = typeData.TypeFullNameWithoutGenericParameters;

                        var lastIndexOfDot = typeFullNameWithoutGenericParameters.LastIndexOf('.');
                        if (lastIndexOfDot <= 0)
                            return null;

                        internalTypeNameStrBldr.Append($"{typeFullNameWithoutGenericParameters.Substring(0, lastIndexOfDot)}+{typeFullNameWithoutGenericParameters.Substring(lastIndexOfDot + 1)}");
                    }
                    else
                    {
                        internalTypeNameStrBldr.Append(typeData.TypeFullNameWithoutGenericParameters);
                    }

                    if (genericTypeParametersList.Count > 0)
                        internalTypeNameStrBldr.Append($"`{genericTypeParametersList.Count}");

                    var internalTypeName = internalTypeNameStrBldr.ToString();

                    foreach (var currAssembly in assembliesToSearch)
                    {
                        System.Reflection.Assembly currAssemblyToLoadTypeFrom;

                        if (currAssembly == assembly)
                        {
                            currAssemblyToLoadTypeFrom = assemblyToLoadTypeFrom;
                        }
                        else
                        {
                            if (currAssembly is IAssembly assemblyElement)
                            {
                                currAssemblyToLoadTypeFrom = LoadAssembly(assemblyElement);
                            }
                            else
                            {
                                currAssemblyToLoadTypeFrom = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x =>
                                    currAssembly.Name.Equals(x.GetName().Name, StringComparison.OrdinalIgnoreCase));

                                if (currAssemblyToLoadTypeFrom == null)
                                {
                                    throw new ConfigurationParseException(requestingConfigurationFileElement,
                                        $"Assembly '{currAssembly.Name}' is not loaded. There should be an error in code, since this scenario should never happen.");
                                }
                            }
                        }

                        var loadedType = currAssemblyToLoadTypeFrom.GetType(internalTypeName, false, false);

                        if (loadedType != null)
                        {
                            if (genericTypeParametersList.Count > 0)
                            {
                                internalTypeNameStrBldr.Append('[');
                                for (var i = 0; i < genericTypeParametersList.Count; ++i)
                                {
                                    var genericTypeParameterType = genericTypeParametersList[i];

                                    if (i > 0)
                                        internalTypeNameStrBldr.Append(", ");

                                    internalTypeNameStrBldr.AppendFormat($"[{genericTypeParameterType.TypeInternalFullNameWithAssembly}]");
                                }

                                internalTypeNameStrBldr.Append(']');
                                internalTypeName = internalTypeNameStrBldr.ToString();

                                loadedType = currAssemblyToLoadTypeFrom.GetType(internalTypeName, false, false);
                            }

                            if (loadedType != null)
                            {
                                if (typeData.IsArray)
                                    return TypeInfo.CreateArrayTypeInfo(loadedType, currAssembly, genericTypeParametersList);
                                return TypeInfo.CreateNonArrayTypeInfo(loadedType, currAssembly, genericTypeParametersList);
                            }
                        }
                    }

                    return null;
                }

                typeInfo = tryFindTypeInAssemblies(false);

                if (typeInfo == null)
                    typeInfo = tryFindTypeInAssemblies(true);

                if (typeInfo == null)
                {
                    var searchedAssembliesDetails = new StringBuilder();

                    searchedAssembliesDetails.Append($"Type '{typeFullNameWithGenericParameters}' was not found");

                    if (assembly != null)
                    {
                        searchedAssembliesDetails.AppendLine($" in assembly '{assembly.Name}' with alias '{assembly.Alias}' at the path '{assembly.AbsolutePath}'.");
                        searchedAssembliesDetails.AppendLine($"To fix the problem, either specify a correct assembly, or remove the attribute '{assemblyAttributeName}' from element '{requestingConfigurationFileElement.ElementName}' to look for the type in all assemblies specified in element 'iocConfiguration/{ConfigurationFileElementNames.Assemblies}'.");
                    }
                    else
                    {
                        searchedAssembliesDetails.AppendLine(".");
                        searchedAssembliesDetails.AppendLine($"Make sure that the assembly where the type is defined is included in element iocConfiguration/{ConfigurationFileElementNames.Assemblies}'.");
                        searchedAssembliesDetails.Append(GetSearchedAssembliesText(assembliesToSearch));
                    }

                    throw new ConfigurationParseException(requestingConfigurationFileElement, searchedAssembliesDetails.ToString());
                }

                _typeFullNameToTypeInfo[typeFullNameWithGenericParameters] = typeInfo;

                LogHelper.Context.Log.Info($"Loaded type '{typeInfo.TypeCSharpFullName}' from assembly '{typeInfo.Assembly.Name}' at '{typeInfo.Type.Assembly.Location}'.");
            }

            if (assembly != null && !typeInfo.Assembly.Name.Equals(assembly.Name))
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine($"Type '{typeInfo.TypeCSharpFullName}' was found in assembly '{typeInfo.Assembly.Name}'.");

                errorMessage.AppendLine($"The type is expected to be in assembly '{assembly.Name}'.");
                errorMessage.AppendLine($"Either remove the attribute '{assemblyAttributeName}' from element, or make sure that the type is defined only in assembly '{assembly.Name}' at '{assembly.AbsolutePath}'.");

                throw new ConfigurationParseException(requestingConfigurationFileElement, errorMessage.ToString());
            }

            return typeInfo;
        }

        private ITypeInfo GetTypeInfoFromTypeFullName([NotNull] IConfigurationFileElement requestingConfigurationFileElement,
                                                      [NotNull] string typeFullName,
                                                      [NotNull] [ItemNotNull] IEnumerable<ITypeInfo> genericTypeParameters,
                                                      [CanBeNull] string typeAttributeName,
                                                      [CanBeNull] string assemblyAttributeName)
        {
            ITypeData typeData = null;

            try
            {
                typeData = _typeParser.Parse(typeFullName);
            }
            catch (ParseTypeException e)
            {
                var errorMessage = new StringBuilder();

                errorMessage.AppendLine($"Failed to parse the value of '{typeFullName}' into a valid type name.");

                string errorLocationText;
                if (typeAttributeName != null)
                {
                    errorLocationText = $"Error location: {typeAttributeName}=\"";
                    errorMessage.AppendLine($"{errorLocationText}{typeFullName}\"");
                }
                else
                {
                    errorLocationText = "Error location: ";
                    errorMessage.AppendLine($"{errorLocationText}{typeFullName}");
                }

                errorMessage.Append(new string('_', errorLocationText.Length + e.ErrorIndex));
                // 2191=up arrow. See https://www.key-shortcut.com/en/writing-systems/35-symbols/arrows/
                errorMessage.AppendLine($"{'\u2191'}");

                errorMessage.AppendLine(e.Message);

                throw new ConfigurationParseException(requestingConfigurationFileElement, errorMessage.ToString());
            }

            List<ITypeInfo> genericTypeParametersList;

            ITypeInfo typeInfo;
            if (typeData.GenericTypeParameters.Count == 0)
            {
                genericTypeParametersList = new List<ITypeInfo>(genericTypeParameters ?? Enumerable.Empty<ITypeInfo>());
                typeInfo = GetTypeInfo2(requestingConfigurationFileElement, typeData, assemblyAttributeName, genericTypeParametersList);
            }
            else
            {
                if (genericTypeParameters != null && genericTypeParameters.Any())
                    throw new ConfigurationParseException(requestingConfigurationFileElement, "Child elements with type parameters are not allowed if the type full name already uses generic type parameters.");

                typeInfo = GetTypeInfoRecursivelly(requestingConfigurationFileElement, typeData, assemblyAttributeName);
            }

            if (typeInfo == null)
                throw new ConfigurationParseException(requestingConfigurationFileElement, "Failed to load the type. We should never get here, since an exception should have been thrown before we get here.");

            _pluginAssemblyTypeUsageValidator.Validate(requestingConfigurationFileElement, typeInfo);

            return typeInfo;
        }

        private ITypeInfo GetTypeInfoRecursivelly([NotNull] IConfigurationFileElement requestingConfigurationFileElement,
                                                  [NotNull] ITypeData typeData,
                                                  [NotNull] string assemblyAttributeName)
        {
            var typeParameters = new List<ITypeInfo>();

            foreach (var typeParameterData in typeData.GenericTypeParameters)
            {
                var typeInfo = GetTypeInfoRecursivelly(requestingConfigurationFileElement, typeParameterData, null);
                typeParameters.Add(typeInfo);
            }

            return GetTypeInfo2(requestingConfigurationFileElement, typeData, assemblyAttributeName, typeParameters);
        }

        private System.Reflection.Assembly LoadAssembly([NotNull] IAssembly assembly)
        {
            try
            {
                return _assemblyLocator.LoadAssembly(Path.GetFileName(assembly.AbsolutePath), Path.GetDirectoryName(assembly.AbsolutePath));
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Error(e);
                throw new ConfigurationParseException(assembly, $"Failed to load assembly '{assembly.AbsolutePath}'.");
            }
        }

        private bool TryGetPrimitiveType(IConfigurationFileElement requestingConfigurationFileElement, out ITypeInfo typeInfo)
        {
            var elementName = requestingConfigurationFileElement.ElementName;

            if (elementName.Equals(ConfigurationFileElementNames.ValueByte, StringComparison.OrdinalIgnoreCase))
            {
                return GetPrimitiveTypeInfo<byte>(requestingConfigurationFileElement.Configuration, out typeInfo);
            }

            if (elementName.Equals(ConfigurationFileElementNames.ValueInt16, StringComparison.OrdinalIgnoreCase))
            {
                return GetPrimitiveTypeInfo<short>(requestingConfigurationFileElement.Configuration, out typeInfo);
            }

            if (elementName.Equals(ConfigurationFileElementNames.ValueInt32, StringComparison.OrdinalIgnoreCase))
            {
                return GetPrimitiveTypeInfo<int>(requestingConfigurationFileElement.Configuration, out typeInfo);
            }

            if (elementName.Equals(ConfigurationFileElementNames.ValueInt64, StringComparison.OrdinalIgnoreCase))
            {
                return GetPrimitiveTypeInfo<long>(requestingConfigurationFileElement.Configuration, out typeInfo);
            }

            if (elementName.Equals(ConfigurationFileElementNames.ValueDouble, StringComparison.OrdinalIgnoreCase))
            {
                return GetPrimitiveTypeInfo<double>(requestingConfigurationFileElement.Configuration, out typeInfo);
            }

            if (elementName.Equals(ConfigurationFileElementNames.ValueBoolean, StringComparison.OrdinalIgnoreCase))
            {
                return GetPrimitiveTypeInfo<bool>(requestingConfigurationFileElement.Configuration, out typeInfo);
            }

            if (elementName.Equals(ConfigurationFileElementNames.ValueString, StringComparison.OrdinalIgnoreCase))
            {
                return GetPrimitiveTypeInfo<string>(requestingConfigurationFileElement.Configuration, out typeInfo);
            }

            if (elementName.Equals(ConfigurationFileElementNames.ValueDateTime, StringComparison.OrdinalIgnoreCase))
            {
                return GetPrimitiveTypeInfo<DateTime>(requestingConfigurationFileElement.Configuration, out typeInfo);
            }

            typeInfo = null;
            return false;
        }

        #endregion
    }
}