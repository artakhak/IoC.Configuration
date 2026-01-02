using OROptimizer.Diagnostics.Log;

namespace WebApiDemo.Startup;

public class LogHelperContextLogToConsole: LogHelperContextAbstr
{
    protected override ILog CreateLog(Type typeThatOwnsTheLog)
    {
        return new LogToConsole(OROptimizer.Diagnostics.Log.LogLevel.Debug);
    }
}