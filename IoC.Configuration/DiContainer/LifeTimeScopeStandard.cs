namespace IoC.Configuration.DiContainer
{
    public class LifeTimeScopeStandard : ILifeTimeScope
    {
        #region ILifeTimeScope Interface Implementation

        public event LifeTimeScopeTerminatedEventHandler LifeTimeScopeTerminated;

        #endregion

        #region Current Type Interface

        public virtual void Dispose()
        {
            LifeTimeScopeTerminated?.Invoke(this, new LifeTimeScopeTerminatedEventArgs(this));
        }

        #endregion
    }
}