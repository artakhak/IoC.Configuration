using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IParameterSerializers : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        ITypeBasedSimpleSerializerAggregator TypeBasedSimpleSerializerAggregator { get; }

        #endregion
    }
}