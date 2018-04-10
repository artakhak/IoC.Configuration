using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface ITypeFactoryReturnedTypesSelector : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<ITypeFactoryReturnedType> ReturnedTypes { get; }

        #endregion
    }
}