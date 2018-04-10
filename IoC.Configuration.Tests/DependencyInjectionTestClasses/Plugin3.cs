using System.Collections.Generic;

namespace IoC.Configuration.Tests.DependencyInjectionTestClasses
{
    public class Plugin3 : IPlugin
    {
        #region  Constructors

        public Plugin3()
        {
            RequiredSettings = new List<SettingInfo>();
        }

        #endregion

        #region IPlugin Interface Implementation

        public void Dispose()
        {
        }

        public void Initialize()
        {
        }

        public IPluginData PluginData { get; set; }
        public IEnumerable<SettingInfo> RequiredSettings { get; }

        #endregion
    }
}