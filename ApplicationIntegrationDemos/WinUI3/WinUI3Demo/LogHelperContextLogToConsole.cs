using System;
using OROptimizer.Diagnostics.Log;

namespace WinUI3Demo;

public class LogHelperContextLogToConsole: LogHelperContextAbstr
{
    protected override ILog CreateLog(Type typeThatOwnsTheLog)
    {
        return new LogToConsole(OROptimizer.Diagnostics.Log.LogLevel.Debug);
    }
}