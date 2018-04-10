using IoC.Configuration.DiContainer;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using IoCConfiguration = IoC.Configuration;

namespace Modules.IoC
{
    public class DiModule1 : ModuleAbstr
    {
        #region  Constructors

        public DiModule1(int param1)
        {
            Property1 = param1;
        }

        #endregion

        #region Member Functions

        protected override void AddServiceRegistrations()
        {
            Bind<IInterface1>().To<Interface1_Impl3>().SetResolutionScope(DiResolutionScope.Transient);
            Bind<IInterface6>().OnlyIfNotRegistered().To<Interface6_Impl1>().SetResolutionScope(DiResolutionScope.Singleton);
        }

        public int Property1 { get; }

        #endregion
    }
}