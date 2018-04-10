using Ninject.Modules;
using SharedServices.Interfaces;
using TestPluginAssembly1.Implementations;

namespace ModulesForPlugin1.Ninject
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

        public override void Load()
        {
            Bind<IInterface1>().To<Interface1_Impl2>().InSingletonScope();
        }

        public int Property1 { get; }

        #endregion
    }
}