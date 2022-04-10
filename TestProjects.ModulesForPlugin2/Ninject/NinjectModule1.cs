using Ninject.Modules;

namespace ModulesForPlugin2.Ninject
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
            // Does not register any bindings for now.
        }

        public int Property1 { get; }

        #endregion
    }
}