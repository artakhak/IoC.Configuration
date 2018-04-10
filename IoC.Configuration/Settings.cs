using System;
using System.Collections.Generic;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    public class Settings : ISettings
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<string, ISetting> _settingNameToSettingMap = new Dictionary<string, ISetting>(StringComparer.OrdinalIgnoreCase);

        [NotNull]
        private readonly ITypeBasedSimpleSerializerAggregator _typeBasedSimpleSerializerAggregator;

        #endregion

        #region  Constructors

        public Settings([NotNull] ISettingsElement settingsElement, [NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            _typeBasedSimpleSerializerAggregator = typeBasedSimpleSerializerAggregator;

            foreach (var setting in settingsElement.AllSettings)
                _settingNameToSettingMap[setting.Name] = new Setting(setting);
        }

        #endregion

        #region ISettings Interface Implementation

        public IEnumerable<ISetting> AllSettings => _settingNameToSettingMap.Values;

        public ISetting GetSetting(string name)
        {
            return _settingNameToSettingMap.TryGetValue(name, out var setting) ? setting : null;
        }

        public bool GetSettingValue<T>(string name, T defaultValue, out T value)
        {
            var setting = GetSetting(name);
            value = defaultValue;

            if (setting != null)
            {
                var convertedToType = typeof(T);
                if (!convertedToType.IsAssignableFrom(setting.ValueType))
                {
                    var errorMessage = string.Format("Trying to convert the value of setting '{0}' of type '{1}' to a wrong type '{2}'.",
                        setting.Name, setting.ValueType, convertedToType);

                    if (_typeBasedSimpleSerializerAggregator.TryDeserialize(setting.ValueAsString, defaultValue, out var deserializedValue))
                    {
                        value = deserializedValue;
                        LogHelper.Context.Log.WarnFormat(errorMessage);
                    }
                    else
                    {
                        value = defaultValue;
                        LogHelper.Context.Log.ErrorFormat(errorMessage);
                    }

                    return false;
                }

                if (setting.DeserializedValue != null)
                    value = (T) setting.DeserializedValue;

                return true;
            }

            return false;
        }

        public T GetSettingValueOrThrow<T>(string name)
        {
            var setting = GetSetting(name);

            if (setting == null)
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                    $"Setting '{name}' does not exist in settings section.");
            //throw new Exception($"Setting '{name}' does not exist in settings section.");

            if (setting.ValueType != typeof(T))
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                    $"The setting value type for setting '{name}' is '{setting.ValueType.FullName}'. Expected setting value type is '{typeof(T).FullName}'.");

            T settingValue;

            if (!GetSettingValue(name, default(T), out settingValue))
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                    $"Failed to retrieve the value of setting '{name}'.");

            return settingValue;
        }

        #endregion
    }
}