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
using JetBrains.Annotations;

namespace IoC.Configuration
{
    /// <summary>
    ///     Settings in element iocCOnfiguration/settings in configuration file.
    /// </summary>
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
        ///     Returns an object of type <see cref="ISetting" /> with data loaded from a setting in configuration file.
        /// </summary>
        /// <param name="name">Setting name in configuration file.</param>
        /// <returns>
        ///     Returns an instance of <see cref="ISetting" />, if there is a setting named <paramref name="name" />.
        ///     Returns null otherwise.
        /// </returns>
        [CanBeNull]
        ISetting GetSetting([NotNull] string name);

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