using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public class InjectedPropertyElement : ParameterElement, IInjectedPropertyElement
    {
        #region  Constructors

        public InjectedPropertyElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent, [NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator, [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent, typeBasedSimpleSerializerAggregator, assemblyLocator)
        {
        }

        #endregion
    }
}