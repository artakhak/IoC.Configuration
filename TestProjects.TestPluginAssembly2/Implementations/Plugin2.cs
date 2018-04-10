using SharedServices.Implementations;

namespace TestPluginAssembly2.Implementations
{
    public class Plugin2 : PluginBaseForTests
    {
        #region  Constructors

        public Plugin2(bool param1, double param2, string param3)
        {
            Property1 = param1;
            Property2 = param2;
            Property3 = param3;
        }

        #endregion

        #region Member Functions

        public bool Property1 { get; }
        public double Property2 { get; set; }
        public string Property3 { get; }

        #endregion
    }
}