using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.Tests.ProvidedValue.TestClasses;

public interface ITestInterface1
{
    ILog LoggerInjectedInConstructor { get; }
    ILog LoggerInjectedIntoProperty { get; }
}