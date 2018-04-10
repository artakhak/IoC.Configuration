using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IAssemblies : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<IAssembly> AllAssemblies { get; }

        [CanBeNull]
        IAssembly GetAssemblyByAlias([NotNull] string alias);

        #endregion
    }
}