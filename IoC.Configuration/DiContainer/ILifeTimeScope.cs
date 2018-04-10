using System;

namespace IoC.Configuration.DiContainer
{
    public interface ILifeTimeScope : IDisposable
    {
        #region Current Type Interface

        event LifeTimeScopeTerminatedEventHandler LifeTimeScopeTerminated;

        #endregion
    }
}