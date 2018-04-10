using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IParameterSerializer : IConfigurationFileElement
    {
        #region Current Type Interface

        /// <summary>
        ///     Can be null only if Enabled is false.
        /// </summary>
        [CanBeNull]
        ITypeBasedSimpleSerializer Serializer { get; }

        #endregion
    }
}