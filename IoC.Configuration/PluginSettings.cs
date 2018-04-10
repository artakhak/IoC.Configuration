using System;
using System.Collections.Generic;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    public class PluginSettings : ISettings
    {
        #region Member Variables

        [NotNull]
        private readonly ISettings _globalSettings;

        [NotNull]
        private readonly ISettings _pluginSettings;

        [NotNull]
        private readonly Dictionary<string, ISetting> _settingNameToSettingMap = new Dictionary<string, ISetting>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region  Constructors

        public PluginSettings([NotNull] ISettings globalSettings, ISettingsElement pluginSettingsElementElement,
                              [NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            _globalSettings = globalSettings;
            _pluginSettings = new Settings(pluginSettingsElementElement, typeBasedSimpleSerializerAggregator);

            foreach (var setting in _globalSettings.AllSettings)
                _settingNameToSettingMap[setting.Name] = setting;

            // Override global settings with plugin settings
            foreach (var setting in _pluginSettings.AllSettings)
                _settingNameToSettingMap[setting.Name] = setting;
        }

        #endregion

        #region ISettings Interface Implementation

        public IEnumerable<ISetting> AllSettings => _settingNameToSettingMap.Values;

        public ISetting GetSetting(string name)
        {
            var setting = _pluginSettings.GetSetting(name);

            if (setting != null)
                return setting;

            return _globalSettings.GetSetting(name);
        }

        public bool GetSettingValue<T>(string name, T defaultValue, out T value)
        {
            if (_pluginSettings.GetSettingValue(name, defaultValue, out value))
                return true;

            return _globalSettings.GetSettingValue(name, defaultValue, out value);
        }

        public T GetSettingValueOrThrow<T>(string name)
        {
            var setting = _pluginSettings.GetSetting(name);

            var settingValueType = typeof(T);

            // If the setting is in plugin settings, use it
            if (setting?.ValueType == settingValueType)
                return _pluginSettings.GetSettingValueOrThrow<T>(name);

            return _globalSettings.GetSettingValueOrThrow<T>(name);
        }

        #endregion
    }
}