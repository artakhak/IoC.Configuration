using System;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public class LifeTimeScopeTerminatedEventArgs : EventArgs
    {
        #region  Constructors

        public LifeTimeScopeTerminatedEventArgs([NotNull] ILifeTimeScope lifeTimeScope)
        {
            LifeTimeScope = lifeTimeScope;
        }

        #endregion

        #region Member Functions

        [NotNull]
        public ILifeTimeScope LifeTimeScope { get; }

        #endregion
    }
}