using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class Interface14_Impl1 : IInterface14
    {
        public IInterface13 InterfaceInjectedValue { get; }
        public Interface13_Impl1 NonInterfaceInjectedValue { get; }

        public Interface14_Impl1(IInterface13 interfaceInjectedValue, Interface13_Impl1 nonInterfaceInjectedValue)
        {
            InterfaceInjectedValue = interfaceInjectedValue;
            NonInterfaceInjectedValue = nonInterfaceInjectedValue;
        }
    }
}