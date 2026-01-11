using System.Xml;
using IoC.Configuration.DiContainerBuilder.FileBased;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public class ProvidedValueInitializerElement: ValueInitializerElement
    {
        private readonly IValueProviderWithCachedValuesForValueInitializerElements _valueProviderWithCachedValuesForValueInitializerElements;
        private readonly string _name;
        private readonly ProvidedValueTargetType _providedValueTargetType;
        
        public ProvidedValueInitializerElement([NotNull] XmlElement xmlElement, 
            IConfigurationFileElement parent, [NotNull] ITypeHelper typeHelper, 
            IValueProviderWithCachedValuesForValueInitializerElements valueProviderWithCachedValuesForValueInitializerElements, 
            string name, ProvidedValueTargetType providedValueTargetType
            ) : base(xmlElement, parent, typeHelper)
        {
            _valueProviderWithCachedValuesForValueInitializerElements = valueProviderWithCachedValuesForValueInitializerElements;
            _name = name;
            _providedValueTargetType = providedValueTargetType;
        }

        /// <inheritdoc />
        protected override string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
#pragma warning disable CS0612, CS0618
            return string.Concat(
                FileBasedConfiguration.DynamicImplementationsNamespaceStatic,
                ".",
                DynamicCodeGenerationHelpers.IoCConfigurationContextDataClassName,
                ".",
                DynamicCodeGenerationHelpers.GetValueProviderWithCachedValuesForValueInitializerElementsPropertyName(),
                "().",
                nameof(IValueProviderWithCachedValuesForValueInitializerElements.ResolveValue),
                "<",
                ValueTypeInfo.TypeCSharpFullName,
                ">(",
                this.GetType().GUID.ToString("N"),
                ");"
            );
#pragma warning restore CS0612, CS0618
        }

        /// <inheritdoc />
        public override object GenerateValue()
        {
            return _valueProviderWithCachedValuesForValueInitializerElements.ResolveValue<object>(this.GetType().GUID);
        }

        /// <inheritdoc />
        public override bool IsResolvedFromDiContainer => false;

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            if (this.ValueTypeInfo == null)
                throw new ConfigurationParseException(this,
                    $"The type information property [{nameof(ValueTypeInfo)}] was not initialized.");
            
            var providedValueData = new ProvidedValueData(ValueTypeInfo.Type, _name, _providedValueTargetType);
            if (_valueProviderWithCachedValuesForValueInitializerElements.TryResolveValue(
                    this.GetType().GUID, providedValueData, out object providedValue) && providedValue != null)
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