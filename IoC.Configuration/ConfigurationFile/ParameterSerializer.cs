using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public class ParameterSerializer : ObjectInstanceElementAbstr<ITypeBasedSimpleSerializer>, IParameterSerializer
    {
        #region  Constructors

        public ParameterSerializer([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent, [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent, assemblyLocator)
        {
        }

        #endregion

        #region IParameterSerializer Interface Implementation

        public ITypeBasedSimpleSerializer Serializer => Instance;

        #endregion
    }
}