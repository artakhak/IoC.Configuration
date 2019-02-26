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
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    /// <summary>
    ///     Determines collection type based on context where the element is used
    /// </summary>
    /// <seealso cref="CollectionValueElementBase" />
    public class ContextBasedCollectionValueElement : CollectionValueElementBase
    {
        #region Member Variables

        [NotNull]
        private readonly ICanHaveCollectionChildElement _parentElement;

        #endregion

        #region  Constructors

        public ContextBasedCollectionValueElement([NotNull] XmlElement xmlElement, [NotNull] ICanHaveCollectionChildElement parentElement,
                                                  [NotNull] ITypeHelper typeHelper,
                                                  [NotNull] IPluginAssemblyTypeUsageValidator pluginAssemblyTypeUsageValidator) : base(xmlElement, parentElement, typeHelper, pluginAssemblyTypeUsageValidator)
        {
            _parentElement = parentElement;
        }

        #endregion

        #region Member Functions

        protected override (ITypeInfo itemTypeInfo, CollectionType collectionType) GetCollectionInitializationData()
        {
            ITypeInfo itemTypeInfo = null;
            var collectionType = CollectionType.Enumerable;

            var collectionTypeInfo = _parentElement.ExpectedChildTypeInfo;

            bool tryGetApplicableCollectionType(Type typeToTry)
            {
                return collectionTypeInfo.Type.IsAssignableFrom(typeToTry);
            }

            if (collectionTypeInfo.ArrayItemTypeInfo != null)
            {
                itemTypeInfo = collectionTypeInfo.ArrayItemTypeInfo;

                if (tryGetApplicableCollectionType(collectionTypeInfo.Type))
                {
                    collectionType = CollectionType.Array;
                }
            }
            else if (collectionTypeInfo.GenericTypeParameters.Count == 1)
            {
                itemTypeInfo = collectionTypeInfo.GenericTypeParameters[0];

                if (tryGetApplicableCollectionType(
                    Type.GetType($"System.Collections.Generic.IEnumerable`1[[{itemTypeInfo.TypeInternalFullNameWithAssembly}]]")))
                {
                    collectionType = CollectionType.Enumerable;
                }
                else if (tryGetApplicableCollectionType(
                    Type.GetType($"System.Collections.Generic.IReadOnlyList`1[[{itemTypeInfo.TypeInternalFullNameWithAssembly}]]")))
                {
                    collectionType = CollectionType.ReadOnlyList;
                }
                else if (tryGetApplicableCollectionType(
                    Type.GetType($"System.Collections.Generic.List`1[[{itemTypeInfo.TypeInternalFullNameWithAssembly}]]")))
                {
                    collectionType = CollectionType.List;
                }
                else
                {
                    itemTypeInfo = null;
                }
            }

            if (itemTypeInfo == null)
                throw new ConfigurationParseException(this, $"Type '{collectionTypeInfo.TypeCSharpFullName}' cannot be converted to collection type.", _parentElement);

            return (itemTypeInfo, collectionType);
        }

        #endregion
    }
}