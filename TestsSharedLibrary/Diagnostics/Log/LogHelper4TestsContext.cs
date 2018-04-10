using System;
using OROptimizer.Diagnostics.Log;

namespace TestsSharedLibrary.Diagnostics.Log
{
    public class LogHelper4TestsContext : LogHelperContextAbstr
    {
        #region Member Functions

        protected override ILog CreateLog(Type typeThatOwnsTheLog)
        {
            return new Log4Tests();
        }

        #endregion
    }
}