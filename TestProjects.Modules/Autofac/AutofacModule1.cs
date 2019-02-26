using Autofac;
using IoC.Configuration.DiContainer;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using AutofacModule = Autofac.Module;

namespace Modules.Autofac
{
    public class AutofacModule1 : AutofacModule
    {
        #region  Constructors

        public AutofacModule1(int param1)
        {
            Property1 = param1;
        }

        #endregion

        #region Member Functions

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

        #endregion
    }
}