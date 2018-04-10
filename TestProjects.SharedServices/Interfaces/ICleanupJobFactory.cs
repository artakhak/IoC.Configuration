using System.Collections.Generic;

namespace SharedServices.Interfaces
{
    public interface ICleanupJobFactory
    {
        #region Current Type Interface

        IEnumerable<ICleanupJob> GetCleanupJobs(int projectId);

        #endregion
    }
}