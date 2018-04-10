using System;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder
{
    public interface IContainerInfo : IDisposable
    {
        #region Current Type Interface

        [NotNull]
        IDiContainer DiContainer { get; }

        #endregion
    }
}