using IoC.Configuration.DiContainer;
using SharedServices.Interfaces;
using TestPluginAssembly1.Implementations;
using TestPluginAssembly1.Interfaces;

namespace ModulesForPlugin1.IoC
{
    public class DiModule1 : ModuleAbstr
    {
        #region  Constructors

        public DiModule1(int param1)
        {
            Property1 = param1;
        }

        public DiModule1(IDoor param1)
        {
            Property2 = param1;
        }

        #endregion

        #region Member Functions

        protected override void AddServiceRegistrations()
        {
            Bind<IInterface1>().To<Interface1_Impl3>().SetResolutionScope(DiResolutionScope.Transient);
        }

        public int Property1 { get; }

        public IDoor Property2 { get; }

        #endregion
    }
}