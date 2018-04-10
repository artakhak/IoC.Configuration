namespace SharedServices.Interfaces
{
    public interface ICleanupJob
    {
        #region Current Type Interface

        void Cleanup();

        /// <summary>
        ///     This property is used to test constructor and property injection in automatically implemented factories.
        /// </summary>
        ICleanupJobData CleanupJobData { get; }

        #endregion
    }
}