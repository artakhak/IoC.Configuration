namespace IoC.Configuration.OnApplicationStart
{
    public interface IStartupAction
    {
        #region Current Type Interface

        bool ActionExecutionCompleted { get; }
        void Start();
        void Stop();

        #endregion
    }
}