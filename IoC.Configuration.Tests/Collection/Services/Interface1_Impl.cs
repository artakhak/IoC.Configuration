using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.Collection.Services
{
    public class Interface1_Impl : IInterface1
    {
        public Interface1_Impl(int param1)
        {
            Property1 = param1;
        }
        public int Property1 { get; }
    }
}