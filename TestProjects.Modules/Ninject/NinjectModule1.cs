using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder;
using Ninject.Modules;
using SharedServices.Implementations;
using SharedServices.Interfaces;

namespace Modules.Ninject
{
    public class NinjectModule1 : NinjectModule
    {
        #region  Constructors

        public NinjectModule1(int param1)
        {
            Property1 = param1;
        }

        #endregion

        #region Member Functions

        public IDiContainer DiContainer { get; private set; }

        public override void Load()
        {
            Bind<IInterface1>().To<Interface1_Impl2>().InSingletonScope();
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