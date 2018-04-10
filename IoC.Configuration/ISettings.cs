using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public interface ISettings
    {
        #region Current Type Interface

        /// <summary>
        ///     A collection of all settings loading from configuration file.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        IEnumerable<ISetting> AllSettings { get; }

        /// <summary>
        ///     Returns an object of type <see cref="ISetting" /> with sata loaded from a setting in configuration file.
        /// </summary>
        /// <param name="name">Setting name in configuration file.</param>
        /// <returns></returns>
        [CanBeNull]
        ISetting GetSetting([NotNull] string name);

        /// <summary>
        ///     Gets the value of a setting if the setting is present in configuration file and has the specified type.
        ///     Otherwise, returns the specified default value.
        /// </summary>
        /// <typeparam name="T">Setting type in configuration file.</typeparam>
        /// <param name="name">Setting name in configuration file.</param>
        /// <param name="defaultValue">
        ///     A value to return, if the setting is not in configuration file, or if it is not of
        ///     type <typeparamref name="T" />.
        /// </param>
        /// <param name="value"></param>
        /// <returns>Returns setting value.</returns>
        bool GetSettingValue<T>([NotNull] string name, T defaultValue, out T value);

        /// <summary>
        ///     Gets the value of a setting if the setting is present in configuration file and has the specified type.
        ///     Otherwise, throws an exception.
        /// </summary>
        /// <typeparam name="T">Setting type in configuration file.</typeparam>
        /// <param name="name">Setting name in configuration file</param>
        /// <returns>Returns setting value.</returns>
        /// <exception cref="Exception">Throws this exception.</exception>
        [NotNull]
        T GetSettingValueOrThrow<T>([NotNull] string name);

        #endregion
    }
}