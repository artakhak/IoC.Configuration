using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface ITypeFactoryReturnedTypesIfSelector : ITypeFactoryReturnedTypesSelector
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<string> ParameterValues { get; }

        #endregion
    }
}