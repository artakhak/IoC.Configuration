using IoC.Configuration.DiContainer;
using SharedServices.Implementations;
using SharedServices.Interfaces;

namespace Modules.IoC;

public class DiModule3 : ModuleAbstr
{
    public DiModule3(int diModule3_param1)
    {
        DiModule3_Property1 = diModule3_param1;
    }

    protected override void AddServiceRegistrations()
    {
        Bind<IInterface1>().To<Interface1_Impl3>().SetResolutionScope(DiResolutionScope.Transient);
    }

    public int DiModule3_Property1 { get; }
}