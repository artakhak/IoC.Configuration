using Autofac;
namespace DynamicImplementations_636595161441212446.Plugin2
{
public class PluginServicesModule : Autofac.Module
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
builder.Register(context => new TestPluginAssembly2.Implementations.Wheel(_parameterSerializer.Deserialize<System.Int32>("5"), _parameterSerializer.Deserialize<System.Double>("48"))).As<TestPluginAssembly2.Interfaces.IWheel>().InstancePerDependency();
builder.Register(context => new TestPluginAssembly2.Implementations.Car(_parameterSerializer.Deserialize<TestPluginAssembly2.Interfaces.IWheel>("248,40"))).As<TestPluginAssembly2.Interfaces.ICar>().InstancePerDependency().OnActivated(e =>
{
e.Instance.Wheel1=_parameterSerializer.Deserialize<TestPluginAssembly2.Interfaces.IWheel>("27,45");
e.Instance.Wheel2=e.Context.Resolve<TestPluginAssembly2.Interfaces.IWheel>();
});
}
}
}
