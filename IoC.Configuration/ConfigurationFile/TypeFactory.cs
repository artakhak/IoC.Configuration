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
using System.Reflection;
using System.Xml;
using IoC.Configuration.DynamicCode;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    [Obsolete("Will be removed after 5/31/2019")]
    public class TypeFactory : ConfigurationFileElementAbstr, ITypeFactory, ICanHaveChildElementsThatUsePluginTypeInNonPluginSection
    {
        #region Member Variables

        private ITypeInfo _implementedInterfaceTypeInfo;

        private readonly List<ITypeFactoryReturnedTypesIfSelector> _returnedIfTypeSelectorsForIfCase = new List<ITypeFactoryReturnedTypesIfSelector>();

        [NotNull]
        private readonly ITypeHelper _typeHelper;

        [NotNull]
        private readonly ITypesListFactoryTypeGenerator _typesListFactoryTypeGenerator;

        #endregion

        #region  Constructors

        public TypeFactory([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                           [NotNull] ITypeHelper typeHelper,
                           [NotNull] ITypesListFactoryTypeGenerator typesListFactoryTypeGenerator) : base(xmlElement, parent)
        {
            _typeHelper = typeHelper;
            _typesListFactoryTypeGenerator = typesListFactoryTypeGenerator;
        }

        #endregion

        #region ITypeFactory Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is ITypeFactoryReturnedTypesSelector returnedTypeSelector)
            {
                string errorMessage;

                foreach (var returnedTypeElement in returnedTypeSelector.ReturnedTypes)
                    if (!_typesListFactoryTypeGenerator.ValidateReturnedType(returnedTypeElement.ReturnedType, ReturnedItemsType, out errorMessage))
                        throw new ConfigurationParseException(returnedTypeElement, errorMessage, this);

                if (returnedTypeSelector is ITypeFactoryReturnedTypesIfSelector returnedTypeIfSelector)
                {
                    if (!_typesListFactoryTypeGenerator.ValidateParameterValues(ImplementedMethodInfo, returnedTypeIfSelector.ParameterValues, out errorMessage))
                        throw new ConfigurationParseException(returnedTypeSelector, errorMessage, this);

                    _returnedIfTypeSelectorsForIfCase.Add(returnedTypeIfSelector);
                }
                else
                {
                    ReturnedTypeSelectorForDefaultCase = returnedTypeSelector;
                }
            }
        }

        public override bool Enabled => base.Enabled && ((_implementedInterfaceTypeInfo.Assembly as IAssembly)?.Enabled ?? true);

        public MethodInfo ImplementedMethodInfo { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            LogHelper.Context.Log.Warn($"Element '{ConfigurationFileElementNames.TypeFactory}' is deprecated and might not be supported in the future. Use more flexible element '{ConfigurationFileElementNames.AutoService}' instead.");

            _implementedInterfaceTypeInfo = _typeHelper.GetTypeInfo(this, ConfigurationFileAttributeNames.Interface, ConfigurationFileAttributeNames.Assembly, ConfigurationFileAttributeNames.TypeRef);

            if (!_typesListFactoryTypeGenerator.ValidateImplementedInterface(_implementedInterfaceTypeInfo.Type, out var implementedMethodInfo, out var returnedItemsType,
                out var errorMessage))
                throw new ConfigurationParseException(this, errorMessage);

            if (OwningPluginElement == null)
            {
                if (_implementedInterfaceTypeInfo.Assembly.Plugin != null)
                    throw new ConfigurationParseException(this, $"Type '{_implementedInterfaceTypeInfo.TypeCSharpFullName}' is defined in assembly '{_implementedInterfaceTypeInfo.Assembly}' which belongs to plugin '{_implementedInterfaceTypeInfo.Assembly.Plugin.Name}'. The '{ConfigurationFileElementNames.TypeFactory}' element should be moved under '{ConfigurationFileElementNames.AutoGeneratedServices}' element for plugin '{_implementedInterfaceTypeInfo.Assembly.Plugin.Name}'.");
            }
            else if (_implementedInterfaceTypeInfo.Assembly.Plugin != OwningPluginElement)
            {
                throw new ConfigurationParseException(this, $"Element '{ConfigurationFileElementNames.TypeFactory}' is declared in plugin '{OwningPluginElement.Name}' while type '{_implementedInterfaceTypeInfo.TypeCSharpFullName}' is declared in assembly '{_implementedInterfaceTypeInfo.Assembly}' which does not belong to plugin '{OwningPluginElement.Name}'.");
            }

            ImplementedMethodInfo = implementedMethodInfo;
            ReturnedItemsType = returnedItemsType;
        }

        public IEnumerable<ITypeFactoryReturnedTypesIfSelector> ReturnedIfTypeSelectorsForIfCase => _returnedIfTypeSelectorsForIfCase;
        public Type ReturnedItemsType { get; private set; }
        public ITypeFactoryReturnedTypesSelector ReturnedTypeSelectorForDefaultCase { get; private set; }

        public override void ValidateOnTreeConstructed()
        {
            base.ValidateOnTreeConstructed();

            if (ReturnedTypeSelectorForDefaultCase == null)
                throw new ConfigurationParseException(this, $"Required element '{ConfigurationFileElementNames.TypeFactoryReturnedTypesDefaultSelector}' is missing.");
        }

        #endregion
    }
}