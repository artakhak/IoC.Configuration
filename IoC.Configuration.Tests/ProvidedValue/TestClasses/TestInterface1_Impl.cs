using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.Tests.ProvidedValue.TestClasses;

public class TestInterface1_Impl: ITestInterface1
{
    public TestInterface1_Impl(ILog loggerInjectedInConstructor)
    {
        LoggerInjectedInConstructor = loggerInjectedInConstructor;
    }
    public ILog LoggerInjectedInConstructor { get; }
    public ILog LoggerInjectedIntoProperty { get; set; }
}