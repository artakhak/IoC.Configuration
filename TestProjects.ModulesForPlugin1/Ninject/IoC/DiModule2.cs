using IoC.Configuration.DiContainer;
using TestPluginAssembly1.Interfaces;

namespace ModulesForPlugin1.IoC
{
    public class DiModule2 : ModuleAbstr
    {
        #region  Constructors

        public DiModule2(IDoor param1)
        {
            Property1 = param1;
        }

        #endregion

        #region Member Functions

        protected override void AddServiceRegistrations()
        {
          
        }

        public IDoor Property1 { get; }

        #endregion
    }
}