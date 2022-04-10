using SharedServices.Implementations;

namespace SharedServices.Interfaces
{
    public interface IInterface14
    {
        IInterface13 InterfaceInjectedValue { get; }
        Interface13_Impl1 NonInterfaceInjectedValue { get; }
    }
}