using SharedServices.Interfaces;

namespace DynamicallyLoadedAssembly1.Implementations
{
    public class CleanupJob3 : ICleanupJob
    {
        #region ICleanupJob Interface Implementation

        public void Cleanup()
        {
            // Do something ...
        }

        public ICleanupJobData CleanupJobData { get; set; }

        #endregion
    }
}