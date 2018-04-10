using System.Collections.Generic;
using AssemblyTotestAssemblyResolution;
using IoC.Configuration;
using SharedServices.Implementations;

namespace TestPluginAssembly1.Implementations
{
    public class Plugin1 : PluginBaseForTests
    {
        #region Member Variables

        private readonly List<SettingInfo> _requiredSettings;

        #endregion

        #region  Constructors

        public Plugin1(long param1)
        {
            Property1 = param1;
            _requiredSettings = new List<SettingInfo>();
            _requiredSettings.Add(new SettingInfo("Int32Setting1", typeof(int)));
            _requiredSettings.Add(new SettingInfo("StringSetting1", typeof(string)));
        }

        #endregion

        #region Member Functions

        protected override void InitializeVirtual()
        {
            // This call will result the assembly DynamicallyLoadedAssembly3 to be loaded from a probing path, specified
            // in configuration.
            TestingAssemblyResolution.TestAssemblyResolution();
        }

        public long Property1 { get; }

        public override IEnumerable<SettingInfo> RequiredSettings => _requiredSettings;

        #endregion
    }
}