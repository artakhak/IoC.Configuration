using IoC.Configuration.OnApplicationStart;
using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class StartupActionBaseForTests : IStartupAction, IStartupActionState
    {
        #region IStartupAction Interface Implementation

        public bool ActionExecutionCompleted => (StartupActionState & StartupActionState.StopCalled) == StartupActionState.StopCalled;

        #endregion

        #region IStartupActionState Interface Implementation

        public StartupActionState StartupActionState { get; private set; } = StartupActionState.NotStarted;

        #endregion

        #region Current Type Interface

        public virtual void Start()
        {
            StartupActionState = StartupActionState.StartCalled;
        }

        public virtual void Stop()
        {
            StartupActionState |= StartupActionState.StopCalled;
        }

        #endregion
    }
}