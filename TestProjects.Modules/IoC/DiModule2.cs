using IoC.Configuration.DiContainer;
using SharedServices.Interfaces;

namespace Modules.IoC
{
    public class DiModule2 : ModuleAbstr
    {
        #region  Constructors
        public DiModule2(IInterface1 param1)
        {
            Property1 = param1;
        }

        #endregion

        #region Member Functions

        protected override void AddServiceRegistrations()
        {
        }

        public IInterface1 Property1 { get; }
        #endregion
    }
}