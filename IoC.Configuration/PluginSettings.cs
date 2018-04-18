// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections.Generic;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    /// <summary>
    /// Stored plugin settings loaded from element iocConfiguration/pluginsSetup/pluginSetup/settings in configuration file.
    /// </summary>
    /// <seealso cref="IoC.Configuration.ISettings" />
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
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginSettings"/> class.
        /// </summary>
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
        /// <summary>
        /// A collection of all settings loading from configuration file for the plugin, as well as settings from general settings element,
        /// that are not in element iocConfiguration/pluginsSetup/pluginSetup/settings.
        /// </summary>
        public IEnumerable<ISetting> AllSettings => _settingNameToSettingMap.Values;

        /// <summary>
        /// Returns an object of type <see cref="ISetting" /> with data loaded from a setting in configuration file.
        /// Note, if the setting is not in plugin section for the plugin, the setting will be looked up
        /// in global settings in iocConfiguration/settings as well.
        /// </summary>
        /// <param name="name">Setting name in configuration file.</param>
        /// <returns>
        /// Returns an instance of <see cref="ISetting"/>, if there is a setting named <paramref name="name"/>.
        /// Returns null otherwise.
        /// </returns>
        public ISetting GetSetting(string name)
        {
            var setting = _pluginSettings.GetSetting(name);

            if (setting != null)
                return setting;

            return _globalSettings.GetSetting(name);
        }

        /// <summary>
        /// Gets the value of a setting, if the setting is present in configuration file and has the specified type.
        /// Otherwise, returns the specified default value.
        /// Note, if the setting is not in plugin section for the plugin, the setting will be looked up
        /// in global settings in iocConfiguration/settings as well.
        /// </summary>
        /// <typeparam name="T">Setting type in configuration file.</typeparam>
        /// <param name="name">Setting name in configuration file.</param>
        /// <param name="defaultValue">A value to return, if the setting is not in configuration file, or if it is not of
        /// type <typeparamref name="T" />.</param>
        /// <param name="value">Setting value.</param>
        /// <returns>
        /// Returns true, if setting of type <typeparamref name="T" /> is present in configuration file. Returns false
        /// otherwise.
        /// </returns>
        public bool GetSettingValue<T>(string name, T defaultValue, out T value)
        {
            if (_pluginSettings.GetSettingValue(name, defaultValue, out value))
                return true;

            return _globalSettings.GetSettingValue(name, defaultValue, out value);
        }

        /// <summary>
        /// Gets the value of a setting if the setting is present in configuration file and has the specified type.
        /// Otherwise, throws an exception.
        /// Note, if the setting is not in plugin section for the plugin, the setting will be looked up
        /// in global settings in iocConfiguration/settings as well.
        /// </summary>
        /// <typeparam name="T">Setting type in configuration file.</typeparam>
        /// <param name="name">Setting name in configuration file</param>
        /// <returns>
        /// Returns setting value.
        /// </returns>
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