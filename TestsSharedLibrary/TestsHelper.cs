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
                LogHelper.RegisterContext(new LogHelper4TestsContext());
        }

        #endregion
    }
}