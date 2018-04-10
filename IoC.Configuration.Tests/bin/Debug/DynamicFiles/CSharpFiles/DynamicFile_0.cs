using System;
using Autofac;
using DynamicallyLoadedAssembly1.Implementations;
using DynamicallyLoadedAssembly2;
using IoC.Configuration.DiContainer;
using OROptimizer.Serializer;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using TestPluginAssembly1.Implementations;
using TestPluginAssembly2.Implementations;
using IInterface1 = DynamicallyLoadedAssembly1.Interfaces.IInterface1;
using IInterface2 = DynamicallyLoadedAssembly1.Interfaces.IInterface2;
using IInterface3 = DynamicallyLoadedAssembly1.Interfaces.IInterface3;
using Interface1_Impl1 = DynamicallyLoadedAssembly1.Implementations.Interface1_Impl1;
using Interface2_Impl1 = DynamicallyLoadedAssembly1.Implementations.Interface2_Impl1;
using Interface3_Impl1 = DynamicallyLoadedAssembly1.Implementations.Interface3_Impl1;
using SelfBoundService1 = DynamicallyLoadedAssembly1.Implementations.SelfBoundService1;

namespace DynamicImplementations_636583977886378414
{
    public class ServicesModule : Module
    {
        #region Member Variables

        private IDiContainer _diContainer;
        private ITypeBasedSimpleSerializerAggregator _parameterSerializer;

        #endregion

        #region Member Functions

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<Interface1_Impl1>().As<IInterface1>().SingleInstance();
            builder.RegisterType<Interface2_Impl1>().As<IInterface2>().InstancePerDependency();
            builder.RegisterType<Interface3_Impl1>().As<IInterface3>().InstancePerLifetimeScope();
            builder.RegisterType<Interface9_Impl1>().As<IInterface9>().SingleInstance();
            builder.Register(context => new Interface8_Impl1()).As<IInterface8>().SingleInstance();
            builder.RegisterType<Interface8_Impl2>().As<IInterface8>().SingleInstance();
            builder.Register(context => new SelfBoundService1(_parameterSerializer.Deserialize<int>("14"), _parameterSerializer.Deserialize<double>("15.3"), context.Resolve<IInterface1>())).AsSelf().SingleInstance();
            builder.RegisterType<SelfBoundService2>().AsSelf().InstancePerDependency().OnActivated(e =>
            {
                e.Instance.Property1 = _parameterSerializer.Deserialize<int>("17");
                e.Instance.Property2 = _parameterSerializer.Deserialize<double>("18.1");
                e.Instance.Property3 = e.Context.Resolve<IInterface1>();
            });
            builder.RegisterType<SelfBoundService3>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<SharedServices.Implementations.Interface3_Impl1>().As<SharedServices.Interfaces.IInterface3>().SingleInstance().OnActivated(e => { e.Instance.Property2 = e.Context.Resolve<IInterface4>(); });
            builder.RegisterType<Interface4_Impl1>().As<IInterface4>().SingleInstance();
            builder.Register(context => new SharedServices.Implementations.Interface2_Impl1(_parameterSerializer.Deserialize<DateTime>("2014-10-29 23:59:59.099"), _parameterSerializer.Deserialize<double>("125.1"), context.Resolve<SharedServices.Interfaces.IInterface3>())).As<SharedServices.Interfaces.IInterface2>().SingleInstance();
            builder.RegisterType<Interface2_Impl2>().As<SharedServices.Interfaces.IInterface2>().SingleInstance().OnActivated(e =>
            {
                e.Instance.Property1 = _parameterSerializer.Deserialize<DateTime>("1915-04-24 00:00:00.001");
                e.Instance.Property2 = _parameterSerializer.Deserialize<double>("365.41");
                e.Instance.Property3 = e.Context.Resolve<SharedServices.Interfaces.IInterface3>();
            });
            builder.Register(context => new Interface2_Impl3(_parameterSerializer.Deserialize<DateTime>("2017-10-29 23:59:59.099"), _parameterSerializer.Deserialize<double>("138.3"), context.Resolve<Interface3_Impl2>())).As<SharedServices.Interfaces.IInterface2>().SingleInstance().OnActivated(e =>
            {
                e.Instance.Property2 = _parameterSerializer.Deserialize<double>("148.3");
                e.Instance.Property3 = e.Context.Resolve<Interface3_Impl3>();
            });
            builder.RegisterType<Interface2_Impl4>().As<SharedServices.Interfaces.IInterface2>().SingleInstance();
            builder.Register(context => new ActionValidator3(_parameterSerializer.Deserialize<int>("5"))).AsSelf().InstancePerDependency();
            builder.Register(context => new CleanupJob2(context.Resolve<CleanupJobData2>())).AsSelf().InstancePerDependency();
            builder.RegisterType<CleanupJob3>().AsSelf().SingleInstance().OnActivated(e => { e.Instance.CleanupJobData = e.Context.Resolve<CleanupJobData2>(); });
            builder.RegisterType<CleanupJobData>().As<ICleanupJobData>().SingleInstance();
            builder.RegisterType<Interface5_Impl1>().As<IInterface5>().SingleInstance();
            builder.RegisterType<Interface5_Pluing1Impl>().As<IInterface5>().SingleInstance();
            builder.RegisterType<Interface5_Pluing2Impl>().As<IInterface5>().InstancePerDependency();
            builder.RegisterType<Interface6_Impl2>().As<IInterface6>().IfNotRegistered(typeof(IInterface6)).SingleInstance();
            builder.RegisterType<Interface7_Impl1>().As<IInterface7>().IfNotRegistered(typeof(IInterface7)).SingleInstance();
            builder.RegisterType<SharedServices.Implementations.SelfBoundService1>().AsSelf().IfNotRegistered(typeof(SharedServices.Implementations.SelfBoundService1)).SingleInstance();
        }

        public void OnDiContainerReady(IDiContainer diContainer)
        {
            _diContainer = diContainer;
            _parameterSerializer = _diContainer.Resolve<ITypeBasedSimpleSerializerAggregator>();
        }

        #endregion
    }
}