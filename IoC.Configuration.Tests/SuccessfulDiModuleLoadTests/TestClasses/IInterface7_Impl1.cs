namespace IoC.Configuration.Tests.SuccessfulDiModuleLoadTests.TestClasses
{
    public class Interface7_Impl1 : IInterface7
    {
        public Interface7_Impl1(int property1)
        {
            Property1 = property1;
        }

        public int Property1 { get; }
    }
}