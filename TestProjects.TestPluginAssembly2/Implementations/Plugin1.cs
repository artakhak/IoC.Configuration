using SharedServices.Implementations;

namespace TestPluginAssembly2.Implementations
{
    public class Plugin1 : PluginBaseForTests
    {
        #region  Constructors

        public Plugin1(long param1)
        {
            Property1 = param1;
        }

        #endregion

        #region Member Functions

        public long Property1 { get; }

        #endregion
    }
}