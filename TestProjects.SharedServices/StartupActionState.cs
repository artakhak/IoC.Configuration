using System;
using IoC.Configuration.OnApplicationStart;

namespace SharedServices
{
    /// <summary>
    ///     An enumeration to test if <see cref="IStartupAction.Start" />() and <see cref="IStartupAction.Stop" />() are
    ///     executed.
    /// </summary>
    [Flags]
    public enum StartupActionState
    {
        NotStarted,
        StartCalled = 1,
        StopCalled = 2
    }
}