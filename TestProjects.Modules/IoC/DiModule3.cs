using IoC.Configuration.DiContainer;
using SharedServices.Implementations;
using SharedServices.Interfaces;

namespace Modules.IoC;

public class DiModule3 : ModuleAbstr
{
    public DiModule3(int param1, int param2)
    {
        Property1 = param1;
        Property2 = param2;
    }

    protected override void AddServiceRegistrations()
    {
        Bind<IInterface1>().To<Interface1_Impl3>().SetResolutionScope(DiResolutionScope.Transient);
    }

    public int Property1 { get; }
    public int Property2 { get; }
}