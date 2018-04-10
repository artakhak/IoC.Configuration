using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IParameterSerializersCollection : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<IParameterSerializer> AllSerializers { get; }

        #endregion
    }
}