using System.Collections.Generic;

namespace IoC.Configuration.Tests.SuccessfulDiModuleLoadTests.TestClasses
{
    public class Interface8_Impl1 : IInterface8
    {
        public Interface8_Impl1(IEnumerable<IInterface7> param1)
        {
            Property1 = param1;
        }
        public IEnumerable<IInterface7> Property1 { get; }
    }
}