using System;
using System.Xml;
using IoC.Configuration.DiContainerBuilder.FileBased;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public class ProvidedValueInitializerElement: ValueInitializerElement
    {
        private readonly IValueProviderWithCachedValuesForValueInitializerElements _valueProviderWithCachedValuesForValueInitializerElements;
        private readonly Guid _elementIdentifier = Guid.NewGuid();
        
        public ProvidedValueInitializerElement([NotNull] XmlElement xmlElement, 
            IConfigurationFileElement parent, [NotNull] ITypeHelper typeHelper, 
            IValueProviderWithCachedValuesForValueInitializerElements valueProviderWithCachedValuesForValueInitializerElements) : base(xmlElement, parent, typeHelper)
        {
            _valueProviderWithCachedValuesForValueInitializerElements = valueProviderWithCachedValuesForValueInitializerElements;
        }

        /// <inheritdoc />
        protected override string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            var elementIdentifierString = _elementIdentifier.ToString("N");
#pragma warning disable CS0612, CS0618
            return string.Concat(
                FileBasedConfiguration.DynamicImplementationsNamespaceStatic,
                ".",
                DynamicCodeGenerationHelpers.IoCConfigurationContextDataClassName,
                ".",
                DynamicCodeGenerationHelpers.GetValueProviderWithCachedValuesForValueInitializerElementsPropertyName(),
                ".",
                nameof(IValueProviderWithCachedValuesForValueInitializerElements.ResolveValue),
                "<",
                ValueTypeInfo.TypeCSharpFullName,
                ">(System.Guid.Parse(\"",
                elementIdentifierString,
                "\")",
                ")"
            );
#pragma warning restore CS0612, CS0618
        }

        /// <inheritdoc />
        public override object GenerateValue()
        {
            return _valueProviderWithCachedValuesForValueInitializerElements.ResolveValue<object>(this._elementIdentifier);
        }

        /// <inheritdoc />
        public override bool IsResolvedFromDiContainer => false;

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            if (this.ValueTypeInfo == null)
                throw new ConfigurationParseException(this,
                    $"The type information property [{nameof(ValueTypeInfo)}] was not initialized.");
            
            var parent = this.Parent;
            string name = null;
            ProvidedValueTargetType? providedValueTargetType = null;
            
            if (parent is IParameters)
            {
                name = GetAttributeValue(ConfigurationFileAttributeNames.Name);
                providedValueTargetType = ProvidedValueTargetType.ConstructorParameter;
            }
            else if (parent is ISettingsElement)
            {
                name = GetAttributeValue(ConfigurationFileAttributeNames.Name);
                providedValueTargetType = ProvidedValueTargetType.Setting;
            }
            else if (parent is IInjectedProperties)
            {
                name = GetAttributeValue(ConfigurationFileAttributeNames.Name);
                providedValueTargetType = ProvidedValueTargetType.Property;
            }
            
            var providedValueData = new ProvidedValueData(ValueTypeInfo.Type, name, providedValueTargetType);
            
            if (_valueProviderWithCachedValuesForValueInitializerElements.TryResolveValue(
                    this._elementIdentifier, providedValueData, out object providedValue) && providedValue != null)
            {
                if (providedValueData.Type.IsAssignableFrom(providedValue.GetType()))
                {
                    return;
                }
            }
            
            throw new ConfigurationParseException(this,
                $"Failed to resolve value of type [{providedValueData.Type}] using a value provider [{typeof(IValueProvider)}]. Make sure to register value providers for all types specified in [{ConfigurationFileElementNames.ProvidedValue}] elements.");
        }
    }
}