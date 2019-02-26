using IoC.Configuration.OnApplicationStart;

namespace IoC.Configuration.Tests.ConstructedValue.Services
{
    public class StartupAction1 : IStartupAction
    {
        public StartupAction1(IAppInfo appInfo)
        {
            AppInfo = appInfo;
        }

        public IAppInfo AppInfo { get; }

        /// <summary>
        /// If <c>true</c>, the action was successfully stopped.
        /// </summary>
        public bool ActionExecutionCompleted { get; private set; }

        /// <summary>
        /// Starts the action.
        /// </summary>
        public void Start()
        {
            ActionExecutionCompleted = true;
        }

        /// <summary>
        ///  Stops the action.
        /// </summary>
        public void Stop()
        {
           
        }
    }
}
