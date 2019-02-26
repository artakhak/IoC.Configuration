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
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    /// <summary>
    ///     Settings in element iocCOnfiguration/settings in configuration file.
    /// </summary>
    public class Settings : ISettings
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<string, ISetting> _settingNameToSettingMap = new Dictionary<string, ISetting>(StringComparer.OrdinalIgnoreCase);

        [NotNull]
        private readonly ITypeBasedSimpleSerializerAggregator _typeBasedSimpleSerializerAggregator;

        #endregion

        #region  Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Settings" /> class.
        /// </summary>
        public Settings([NotNull] ISettingsElement settingsElement, [NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            _typeBasedSimpleSerializerAggregator = typeBasedSimpleSerializerAggregator;

            foreach (var setting in settingsElement.AllSettings)
                _settingNameToSettingMap[setting.Name] = new Setting(setting);
        }

        #endregion

        #region ISettings Interface Implementation

        /// <summary>
        ///     A collection of all settings loading from configuration file.
        /// </summary>
        public IEnumerable<ISetting> AllSettings => _settingNameToSettingMap.Values;

        /// <summary>
        ///     Returns an object of type <see cref="ISetting" /> with data loaded from a setting in configuration file.
        /// </summary>
        /// <param name="name">Setting name in configuration file.</param>
        /// <returns>
        ///     Returns an instance of <see cref="ISetting" />, if there is a setting named <paramref name="name" />.
        ///     Returns null otherwise.
        /// </returns>
        public ISetting GetSetting(string name)
        {
            return _settingNameToSettingMap.TryGetValue(name, out var setting) ? setting : null;
        }

        /// <summary>
        ///     Gets the value of a setting, if the setting is present in configuration file and has the specified type.
        ///     Otherwise, returns the specified default value.
        /// </summary>
        /// <typeparam name="T">Setting type in configuration file.</typeparam>
        /// <param name="name">Setting name in configuration file.</param>
        /// <param name="defaultValue">
        ///     A value to return, if the setting is not in configuration file, or if it is not of
        ///     type <typeparamref name="T" />.
        /// </param>
        /// <param name="value">Setting value.</param>
        /// <returns>
        ///     Returns true, if setting of type <typeparamref name="T" /> is present in configuration file. Returns false
        ///     otherwise.
        /// </returns>
        public bool GetSettingValue<T>(string name, T defaultValue, out T value)
        {
            var setting = GetSetting(name);
            value = defaultValue;

            if (setting != null)
            {
                var convertedToType = typeof(T);
                if (!convertedToType.IsTypeAssignableFrom(setting.ValueTypeInfo.Type))
                {
                    LogHelper.Context.Log.ErrorFormat("Trying to convert the value of setting '{0}' of type '{1}' to a wrong type '{2}'.", setting.Name, setting.ValueTypeInfo.Type, convertedToType);

                    return false;
                }

                if (setting.DeserializedValue != null)
                    value = (T) setting.DeserializedValue;

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Gets the value of a setting if the setting is present in configuration file and has the specified type.
        ///     Otherwise, throws an exception.
        /// </summary>
        /// <typeparam name="T">Setting type in configuration file.</typeparam>
        /// <param name="name">Setting name in configuration file</param>
        /// <returns>
        ///     Returns setting value.
        /// </returns>
        public T GetSettingValueOrThrow<T>(string name)
        {
            var setting = GetSetting(name);

            if (setting == null)
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                    $"Setting '{name}' does not exist in settings section.");

            var convertedToType = typeof(T);
            if (!convertedToType.IsTypeAssignableFrom(setting.ValueTypeInfo.Type))
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                    $"The setting value type for setting '{name}' is '{setting.ValueTypeInfo.TypeCSharpFullName}'. Expected setting value type is '{convertedToType.FullName}'.");

            T settingValue;

            if (!GetSettingValue(name, default(T), out settingValue))
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                    $"Failed to retrieve the value of setting '{name}'.");

            return settingValue;
        }

        #endregion
    }
}