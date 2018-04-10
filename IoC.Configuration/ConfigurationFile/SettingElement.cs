using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public class SettingElement : ParameterElement, ISettingElement
    {
        #region  Constructors

        public SettingElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                              [NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator,
                              [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent, typeBasedSimpleSerializerAggregator, assemblyLocator)
        {
        }

        #endregion

        #region ISettingElement Interface Implementation

        public override void Initialize()
        {
            base.Initialize();

            if (ValueInstantiationType == ValueInstantiationType.ResolveFromDiContext)
                throw new ConfigurationParseException(this, $"Settings cannot use '{ConfigurationFileElementNames.ValueInjectedObject}' element.");
        }

        #endregion
    }
}