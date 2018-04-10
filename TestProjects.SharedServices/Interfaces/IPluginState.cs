namespace SharedServices.Interfaces
{
    public interface IPluginState
    {
        #region Current Type Interface

        bool IsDisposedOf { get; }
        bool IsInitialized { get; }

        #endregion
    }
}