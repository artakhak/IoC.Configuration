using System.Collections.Generic;
using System.Linq;

namespace IoC.Configuration
{
    public abstract class PluginAbstr : IPlugin
    {
        #region Member Variables

        private IPluginData _pluginData;
        private bool _pluginDataWasSet;

        #endregion

        #region IPlugin Interface Implementation

        public IPluginData PluginData
        {
            get => _pluginData;
            set
            {
                // Note, we can just check _pluginData for null.
                // However, that way the user can set this property to null, and then change the value to something else.
                // We want to prevent changes to this, once the plugin was initialized from the configuration file.
                if (_pluginDataWasSet)
                    return;

                _pluginDataWasSet = true;
                _pluginData = value;
            }
        }

        #endregion

        #region Current Type Interface

        public abstract void Dispose();

        public abstract void Initialize();
        public virtual IEnumerable<SettingInfo> RequiredSettings => Enumerable.Empty<SettingInfo>();

        #endregion
    }
}