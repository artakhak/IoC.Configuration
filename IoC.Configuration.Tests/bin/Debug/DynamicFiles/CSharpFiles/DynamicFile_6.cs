using Autofac;
namespace DynamicImplementations_636595161441212446
{
public class AdditionalServices : Autofac.Module
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
builder.RegisterType<SharedServices.FakeSettingsRequestor>().As<IoC.Configuration.ISettingsRequestor>().SingleInstance();
builder.RegisterType<DynamicallyLoadedAssembly1.Implementations.StartupAction1>().As<IoC.Configuration.OnApplicationStart.IStartupAction>().SingleInstance();
builder.RegisterType<DynamicallyLoadedAssembly1.Implementations.StartupAction2>().As<IoC.Configuration.OnApplicationStart.IStartupAction>().SingleInstance();
builder.Register(context => new TestPluginAssembly1.Implementations.Plugin1(_parameterSerializer.Deserialize<System.Int64>("25"))).As<IoC.Configuration.IPlugin>().SingleInstance();
builder.Register(context => new TestPluginAssembly2.Implementations.Plugin2(_parameterSerializer.Deserialize<System.Boolean>("true"), _parameterSerializer.Deserialize<System.Double>("25.3"), _parameterSerializer.Deserialize<System.String>("String value"))).As<IoC.Configuration.IPlugin>().SingleInstance().OnActivated(e =>
{
e.Instance.Property2=_parameterSerializer.Deserialize<System.Double>("5.3");
});
}
}
}
