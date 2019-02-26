using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class Interface12_Impl1 : IInterface12
    {
        public Interface12_Impl1(IInterface11 param1)
        {
            Property1 = param1;
        }
        public IInterface11 Property1 { get; }
        public IInterface11 Property2 { get; set; }
    }
}