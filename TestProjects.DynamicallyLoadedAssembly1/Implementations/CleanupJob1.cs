using SharedServices.Interfaces;

namespace DynamicallyLoadedAssembly1.Implementations
{
    public class CleanupJob1 : ICleanupJob
    {
        #region  Constructors

        public CleanupJob1(ICleanupJobData cleanupJobData)
        {
            CleanupJobData = cleanupJobData;
        }

        #endregion

        #region ICleanupJob Interface Implementation

        public void Cleanup()
        {
            // Do something ...
        }

        public ICleanupJobData CleanupJobData { get; }

        #endregion
    }
}