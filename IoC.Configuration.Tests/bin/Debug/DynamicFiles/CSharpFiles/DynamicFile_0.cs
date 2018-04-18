using Autofac;
namespace DynamicImplementations_636595161441212446
{
public class ServicesModule : Autofac.Module
{
private IoC.Configuration.DiContainer.IDiContainer _diContainer;
private OROptimizer.Serializer.ITypeBasedSimpleSerializerAggregator _parameterSerializer;
public void OnDiContainerReady(IoC.Configuration.DiContainer.IDiContainer diContainer)
{
_diContainer=diContainer;
_parameterSerializer = _diContainer.Resolve<OROptimizer.Serializer.ITypeBasedSimpleSerializerAggregator>();
}
protected override void Load(Autofac.ContainerBuilder builder)
{
base.Load(builder);
builder.RegisterType<DynamicallyLoadedAssembly1.Implementations.Interface1_Impl1>().As<DynamicallyLoadedAssembly1.Interfaces.IInterface1>().SingleInstance();
builder.RegisterType<DynamicallyLoadedAssembly1.Implementations.Interface2_Impl1>().As<DynamicallyLoadedAssembly1.Interfaces.IInterface2>().InstancePerDependency();
builder.RegisterType<DynamicallyLoadedAssembly1.Implementations.Interface3_Impl1>().As<DynamicallyLoadedAssembly1.Interfaces.IInterface3>().InstancePerLifetimeScope();
builder.RegisterType<SharedServices.Implementations.Interface9_Impl1>().As<SharedServices.Interfaces.IInterface9>().SingleInstance();
builder.Register(context => new SharedServices.Implementations.Interface8_Impl1()).As<SharedServices.Interfaces.IInterface8>().SingleInstance();
builder.RegisterType<SharedServices.Implementations.Interface8_Impl2>().As<SharedServices.Interfaces.IInterface8>().SingleInstance();
builder.Register(context => new DynamicallyLoadedAssembly1.Implementations.SelfBoundService1(_parameterSerializer.Deserialize<System.Int32>("14"), _parameterSerializer.Deserialize<System.Double>("15.3"), context.Resolve<DynamicallyLoadedAssembly1.Interfaces.IInterface1>())).AsSelf().SingleInstance();
builder.RegisterType<DynamicallyLoadedAssembly1.Implementations.SelfBoundService2>().AsSelf().InstancePerDependency().OnActivated(e =>
{
e.Instance.Property1=_parameterSerializer.Deserialize<System.Int32>("17");
e.Instance.Property2=_parameterSerializer.Deserialize<System.Double>("18.1");
e.Instance.Property3=e.Context.Resolve<DynamicallyLoadedAssembly1.Interfaces.IInterface1>();
});
builder.RegisterType<DynamicallyLoadedAssembly1.Implementations.SelfBoundService3>().AsSelf().InstancePerLifetimeScope();
builder.RegisterType<SharedServices.Implementations.Interface3_Impl1>().As<SharedServices.Interfaces.IInterface3>().SingleInstance().OnActivated(e =>
{
e.Instance.Property2=e.Context.Resolve<SharedServices.Interfaces.IInterface4>();
});
builder.RegisterType<SharedServices.Implementations.Interface4_Impl1>().As<SharedServices.Interfaces.IInterface4>().SingleInstance();
builder.Register(context => new SharedServices.Implementations.Interface2_Impl1(_parameterSerializer.Deserialize<System.DateTime>("2014-10-29 23:59:59.099"), _parameterSerializer.Deserialize<System.Double>("125.1"), context.Resolve<SharedServices.Interfaces.IInterface3>())).As<SharedServices.Interfaces.IInterface2>().SingleInstance();
builder.RegisterType<SharedServices.Implementations.Interface2_Impl2>().As<SharedServices.Interfaces.IInterface2>().SingleInstance().OnActivated(e =>
{
e.Instance.Property1=_parameterSerializer.Deserialize<System.DateTime>("1915-04-24 00:00:00.001");
e.Instance.Property2=_parameterSerializer.Deserialize<System.Double>("365.41");
e.Instance.Property3=e.Context.Resolve<SharedServices.Interfaces.IInterface3>();
});
builder.Register(context => new SharedServices.Implementations.Interface2_Impl3(_parameterSerializer.Deserialize<System.DateTime>("2017-10-29 23:59:59.099"), _parameterSerializer.Deserialize<System.Double>("138.3"), context.Resolve<SharedServices.Implementations.Interface3_Impl2>())).As<SharedServices.Interfaces.IInterface2>().SingleInstance().OnActivated(e =>
{
e.Instance.Property2=_parameterSerializer.Deserialize<System.Double>("148.3");
e.Instance.Property3=e.Context.Resolve<SharedServices.Implementations.Interface3_Impl3>();
});
builder.RegisterType<SharedServices.Implementations.Interface2_Impl4>().As<SharedServices.Interfaces.IInterface2>().SingleInstance();
builder.Register(context => new DynamicallyLoadedAssembly2.ActionValidator3(_parameterSerializer.Deserialize<System.Int32>("5"))).AsSelf().InstancePerDependency();
builder.Register(context => new DynamicallyLoadedAssembly1.Implementations.CleanupJob2(context.Resolve<DynamicallyLoadedAssembly1.Implementations.CleanupJobData2>())).AsSelf().InstancePerDependency();
builder.RegisterType<DynamicallyLoadedAssembly1.Implementations.CleanupJob3>().AsSelf().SingleInstance().OnActivated(e =>
{
e.Instance.CleanupJobData=e.Context.Resolve<DynamicallyLoadedAssembly1.Implementations.CleanupJobData2>();
});
builder.RegisterType<DynamicallyLoadedAssembly1.Implementations.CleanupJobData>().As<SharedServices.Interfaces.ICleanupJobData>().SingleInstance();
builder.RegisterType<SharedServices.Implementations.Interface5_Impl1>().As<SharedServices.Interfaces.IInterface5>().SingleInstance();
builder.RegisterType<TestPluginAssembly1.Implementations.Interface5_Pluing1Impl>().As<SharedServices.Interfaces.IInterface5>().SingleInstance();
builder.RegisterType<TestPluginAssembly2.Implementations.Interface5_Pluing2Impl>().As<SharedServices.Interfaces.IInterface5>().InstancePerDependency();
builder.RegisterType<SharedServices.Implementations.Interface6_Impl2>().As<SharedServices.Interfaces.IInterface6>().IfNotRegistered(typeof(SharedServices.Interfaces.IInterface6)).SingleInstance();
builder.RegisterType<SharedServices.Implementations.Interface7_Impl1>().As<SharedServices.Interfaces.IInterface7>().IfNotRegistered(typeof(SharedServices.Interfaces.IInterface7)).SingleInstance();
builder.RegisterType<SharedServices.Implementations.SelfBoundService1>().AsSelf().IfNotRegistered(typeof(SharedServices.Implementations.SelfBoundService1)).SingleInstance();
}
}
}
