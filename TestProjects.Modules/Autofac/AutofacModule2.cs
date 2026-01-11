using Autofac;
using IoC.Configuration.DiContainer;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using AutofacModule = Autofac.Module;

namespace Modules.Autofac;

public class AutofacModule2 : AutofacModule
{
    public AutofacModule2(int param1, int param2)
    {
        Property1 = param1;
        Property2 = param2;
    }

    public IDiContainer DiContainer { get; private set; }

    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<Interface1_Impl1>().As<IInterface1>().SingleInstance();
    }

    /// <summary>
    ///     The value of parameter <paramref name="diContainer" /> will be injected by <see cref="DiContainerBuilder" />.
    /// </summary>
    /// <param name="diContainer"></param>
    public void OnDiContainerReady(IDiContainer diContainer)
    {
        DiContainer = diContainer;
    }

    public int Property1 { get; }
    public int Property2 { get; }
}