using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class Interface11_Impl1 : IInterface11
    {
        public Interface11_Impl1(IInterface10 param1)
        {
            Property1 = param1;
        }
        public IInterface10 Property1 { get; }
        public IInterface10 Property2 { get; set; }
    }
}