using System;
using JetBrains.Annotations;

namespace IoC.Configuration.OnApplicationStart
{
    /// <summary>
    ///     Implement this class to configure application, such as executing custom code on application start.
    /// </summary>
    public interface IOnApplicationsStarted
    {
        #region Current Type Interface

        void StartStartupActions();
        void StopStartupActions(int maxMillisecondsToWait, [CanBeNull] Action onAllStartupActionsStopped);

        #endregion
    }
}