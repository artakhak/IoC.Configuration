using OROptimizer.Diagnostics.Log;
using TestsSharedLibrary.Diagnostics.Log;

namespace TestsSharedLibrary
{
    public static class TestsHelper
    {
        #region Member Functions

        public static void SetupLogger()
        {
            if (!LogHelper.IsContextInitialized)
                LogHelper.RegisterDefaultContext(new LogHelper4TestsContext());
        }

        #endregion
    }
}