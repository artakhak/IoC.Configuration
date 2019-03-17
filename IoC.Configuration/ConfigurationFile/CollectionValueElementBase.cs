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

using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.DynamicCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class CollectionValueElementBase : ValueInitializerElement, ICollectionValueElement, ICollectionItemValueElement,
                                                       ICanHaveCollectionChildElement,
                                                       ICanHaveChildElementsThatUsePluginTypeInNonPluginSection
    {
        #region Member Variables

        [NotNull]
        private readonly IPluginAssemblyTypeUsageValidator _pluginAssemblyTypeUsageValidator;

        [NotNull]
        [ItemNotNull]
        private readonly List<IValueInitializerElement> _valueInitializerElements = new List<IValueInitializerElement>();

        private ITypeInfo _valueTypeInfo;

        #endregion

        #region  Constructors

        protected CollectionValueElementBase([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                             [NotNull] ITypeHelper typeHelper,
                                             [NotNull] IPluginAssemblyTypeUsageValidator pluginAssemblyTypeUsageValidator) :
            base(xmlElement, parent, typeHelper)
        {
            _pluginAssemblyTypeUsageValidator = pluginAssemblyTypeUsageValidator;
        }

        #endregion

        #region ICanHaveCollectionChildElement Interface Implementation

        ITypeInfo ICanHaveCollectionChildElement.ExpectedChildTypeInfo => ItemTypeInfo;

        #endregion

        #region ICollectionValueElement Interface Implementation
        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            foreach (var child in this.Children)
            {
                if (!(child is IValueInitializerElement valueInitializerElement) || !ItemTypeInfo.Type.IsTypeAssignableFrom(valueInitializerElement.ValueTypeInfo.Type))
                    throw new ConfigurationParseException(child, $"Expects a value of type {ItemTypeInfo.TypeCSharpFullName}.", this);

                var disabledPluginTypeInfo = valueInitializerElement.ValueTypeInfo.GetUniquePluginTypes().FirstOrDefault(x => !x.Assembly.Plugin.Enabled);

                if (disabledPluginTypeInfo == null)
                    _valueInitializerElements.Add(valueInitializerElement);
            }
        }

        public CollectionType CollectionType { get; private set; }

        public override object GenerateValue()
        {
            // https://stackoverflow.com/questions/4661211/c-sharp-instantiate-generic-list-from-reflected-type

            object values = null;

            switch (CollectionType)
            {
                case CollectionType.Array:
                    var array = Array.CreateInstance(ItemTypeInfo.Type, _valueInitializerElements.Count);
                    values = array;
                    for (var i = 0; i < _valueInitializerElements.Count; ++i)
                        array.SetValue(_valueInitializerElements[i].GenerateValue(), i);
                    break;

                case CollectionType.ReadOnlyList:
                case CollectionType.Enumerable:
                case CollectionType.List:
                    var typeToUseForInstantiation = ValueTypeInfo.Type;

                    if (CollectionType != CollectionType.List)
                        typeToUseForInstantiation = Type.GetType(
                            $"System.Collections.Generic.List`1[[{ItemTypeInfo.TypeInternalFullNameWithAssembly}]]");

                    var list = Activator.CreateInstance(typeToUseForInstantiation, _valueInitializerElements.Count);
                    values = list;

                    for (var i = 0; i < _valueInitializerElements.Count; ++i)
                    {
                        var addItemMethodInfo = typeToUseForInstantiation.GetMethod("Add", new[] {ItemTypeInfo.Type});
                        addItemMethodInfo.Invoke(list, new[] {_valueInitializerElements[i].GenerateValue()});
                    }

                    break;
            }

            return values;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is resolved from di container.
        /// </summary>
        public override bool IsResolvedFromDiContainer => false;

        public ITypeInfo ItemTypeInfo { get; private set; }

        #endregion

        #region Current Type Interface

        protected abstract (ITypeInfo itemTypeInfo, CollectionType collectionType) GetCollectionInitializationData();

        #endregion

        #region Member Functions

        protected override string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            var cSharpCode = new StringBuilder();
            cSharpCode.Append("new ");
            switch (CollectionType)
            {
                case CollectionType.Array:
                    cSharpCode.Append($"{ItemTypeInfo.TypeCSharpFullName}[{_valueInitializerElements.Count}] {{");
                    cSharpCode.Append(string.Join(",", _valueInitializerElements.Select(x => x.GenerateValueCSharp(dynamicAssemblyBuilder))));
                    cSharpCode.Append("}");
                    break;

                case CollectionType.Enumerable:
                case CollectionType.ReadOnlyList:
                case CollectionType.List:

                    cSharpCode.Append($"System.Collections.Generic.List<{ItemTypeInfo.TypeCSharpFullName}>({_valueInitializerElements.Count}) {{");
                    cSharpCode.Append(string.Join(",", _valueInitializerElements.Select(x => x.GenerateValueCSharp(dynamicAssemblyBuilder))));
                    cSharpCode.Append("}");
                    break;
            }

            return cSharpCode.ToString();
        }

        protected override ITypeInfo GetValueTypeInfo()
        {
            if (_valueTypeInfo != null)
                return _valueTypeInfo;

            (ItemTypeInfo, CollectionType) = GetCollectionInitializationData();

            switch (CollectionType)
            {
                case CollectionType.Array:
                    _valueTypeInfo = TypeInfo.CreateArrayTypeInfo(ItemTypeInfo.Type, ItemTypeInfo.Assembly, ItemTypeInfo.GenericTypeParameters);
                    break;

                case CollectionType.ReadOnlyList:
                    _valueTypeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType(
                            $"System.Collections.Generic.IReadOnlyList`1[[{ItemTypeInfo.TypeInternalFullNameWithAssembly}]]"),
                        Configuration.Assemblies.MsCorlibAssembly, new[] {ItemTypeInfo});
                    break;

                case CollectionType.Enumerable:
                    _valueTypeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType(
                            $"System.Collections.Generic.IEnumerable`1[[{ItemTypeInfo.TypeInternalFullNameWithAssembly}]]"),
                        Configuration.Assemblies.MsCorlibAssembly, new[] {ItemTypeInfo});
                    break;

                case CollectionType.List:
                    _valueTypeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType(
                            $"System.Collections.Generic.List`1[[{ItemTypeInfo.TypeInternalFullNameWithAssembly}]]"),
                        Configuration.Assemblies.MsCorlibAssembly, new[] {ItemTypeInfo});
                    break;

                default:
                    throw new ConfigurationParseException(this, $"Unrecognized value: {CollectionType}.");
            }

            _pluginAssemblyTypeUsageValidator.Validate(this, ItemTypeInfo);
            return _valueTypeInfo;
        }

        #endregion
    }
}