using System.Linq;
namespace DynamicImplementations_636594305405892785
{
public class ServicesModule : Ninject.Modules.NinjectModule
{
private IoC.Configuration.DiContainer.IDiContainer _diContainer;
private OROptimizer.Serializer.ITypeBasedSimpleSerializerAggregator _parameterSerializer;
public void OnDiContainerReady(IoC.Configuration.DiContainer.IDiContainer diContainer)
{
_diContainer=diContainer;
_parameterSerializer = _diContainer.Resolve<OROptimizer.Serializer.ITypeBasedSimpleSerializerAggregator>();
}
public override void Load()
{
}
}
}
