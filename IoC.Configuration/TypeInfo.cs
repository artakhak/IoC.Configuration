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
using System.Text;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public sealed class TypeInfo : ITypeInfo
    {
        #region Member Variables

        #endregion

        #region  Constructors

        private TypeInfo([NotNull] Type type, bool isArrayOfSpecifiedType, [NotNull] IAssembly assembly, [NotNull] [ItemNotNull] IEnumerable<ITypeInfo> genericTypeParameters)
        {
            var genericTypeParametersList = new List<ITypeInfo>(genericTypeParameters ?? new List<ITypeInfo>(0));

            Assembly = assembly;

            var typeCSharpFullNameStrBldr = new StringBuilder();
            var typeInternalFullNameWithAssemblyStrBldr = new StringBuilder();

            if (genericTypeParametersList.Count == 0)
            {
                typeCSharpFullNameStrBldr.Append(type.FullName.Replace('+', '.'));
                typeInternalFullNameWithAssemblyStrBldr.Append($"{type.FullName}");
            }
            else
            {
                var typeFullNameWithoutGenericParams = type.FullName.Substring(0, type.FullName.IndexOf('`'));

                typeCSharpFullNameStrBldr.Append(typeFullNameWithoutGenericParams.Replace('+', '.'));
                typeCSharpFullNameStrBldr.Append('<');

                typeInternalFullNameWithAssemblyStrBldr.Append(typeFullNameWithoutGenericParams);
                typeInternalFullNameWithAssemblyStrBldr.Append($"`{genericTypeParametersList.Count}[");

                for (var i = 0; i < genericTypeParametersList.Count; ++i)
                {
                    var genericTypeParameterTypeInfo = genericTypeParametersList[i];

                    if (i > 0)
                    {
                        typeCSharpFullNameStrBldr.Append(", ");
                        typeInternalFullNameWithAssemblyStrBldr.Append(", ");
                    }

                    typeInternalFullNameWithAssemblyStrBldr.Append('[');

                    typeCSharpFullNameStrBldr.Append(genericTypeParameterTypeInfo.TypeCSharpFullName);
                    typeInternalFullNameWithAssemblyStrBldr.Append(genericTypeParameterTypeInfo.TypeInternalFullNameWithAssembly);
                    typeInternalFullNameWithAssemblyStrBldr.Append(']');
                }

                typeCSharpFullNameStrBldr.Append('>');
                typeInternalFullNameWithAssemblyStrBldr.Append(']');
            }

            if (isArrayOfSpecifiedType)
            {
                ArrayItemTypeInfo = CreateNonArrayTypeInfo(type, assembly, genericTypeParametersList);
                Type = Array.CreateInstance(type, 0).GetType();
                GenericTypeParameters = new List<ITypeInfo>(0);

                typeCSharpFullNameStrBldr.Append("[]");
                typeInternalFullNameWithAssemblyStrBldr.Append("[]");
            }
            else
            {
                Type = type;
                GenericTypeParameters = genericTypeParametersList;
            }

            typeInternalFullNameWithAssemblyStrBldr.Append($", {assembly.Name}");

            var typeCSharpFullName = typeCSharpFullNameStrBldr.ToString();

            if (typeCSharpFullName[typeCSharpFullName.Length - 1] == '&')
                typeCSharpFullName = typeCSharpFullName.Substring(0, typeCSharpFullName.Length - 1);

            TypeCSharpFullName = typeCSharpFullName;
            TypeInternalFullNameWithAssembly = typeInternalFullNameWithAssemblyStrBldr.ToString();
        }

        #endregion

        #region ITypeInfo Interface Implementation

        public ITypeInfo ArrayItemTypeInfo { get; }
        public IAssembly Assembly { get; }

        [NotNull]
        public IReadOnlyList<ITypeInfo> GenericTypeParameters { get; }

        public IReadOnlyList<ITypeInfo> GetUniquePluginTypes()
        {
            var pluginNameToTypeMap = new Dictionary<string, ITypeInfo>(StringComparer.Ordinal);

            void processTypeInfo(ITypeInfo typeInfoParam, ref bool stopProcessingParam)
            {
                if (typeInfoParam.Assembly.Plugin == null)
                    return;

                if (!pluginNameToTypeMap.ContainsKey(typeInfoParam.Assembly.Plugin.Name))
                    pluginNameToTypeMap[typeInfoParam.Assembly.Plugin.Name] = typeInfoParam;
            }

            var stopProcessing = false;
            ProcessTypeAndGenericParameters(processTypeInfo, ref stopProcessing);

            return new List<ITypeInfo>(pluginNameToTypeMap.Values);
        }

        public void ProcessTypeAndGenericParameters(ProcessTypeInfo typeInfoProcessor, ref bool stopProcessing)
        {
            typeInfoProcessor(this, ref stopProcessing);

            if (stopProcessing)
                return;

            foreach (var generericTypeParameterInfo in GenericTypeParameters)
            {
                generericTypeParameterInfo.ProcessTypeAndGenericParameters(typeInfoProcessor, ref stopProcessing);

                if (stopProcessing)
                    return;
            }
        }

        public Type Type { get; }
        public string TypeCSharpFullName { get; }
        public string TypeInternalFullNameWithAssembly { get; }

        #endregion

        #region Member Functions

        public static TypeInfo CreateArrayTypeInfo([NotNull] Type arrayItemType, [NotNull] IAssembly arrayItemAssembly, [NotNull] [ItemNotNull] IEnumerable<ITypeInfo> arrayItemGenericTypeParameters)
        {
            return new TypeInfo(arrayItemType, true, arrayItemAssembly, arrayItemGenericTypeParameters);
        }

        public static TypeInfo CreateNonArrayTypeInfo([NotNull] Type type, [NotNull] IAssembly assembly, [NotNull] [ItemNotNull] IEnumerable<ITypeInfo> genericTypeParameters)
        {
            return new TypeInfo(type, false, assembly, genericTypeParameters);
        }

        #endregion
    }
}