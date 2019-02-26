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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    /// <summary>
    ///     Default implementation of <see cref="ISettingsRequestor" /> that does not enforce any required settings in
    ///     configuration file.
    ///     Specify your own implementation in element iocConfiguration/settingsRequestor to change the default behaviour.
    /// </summary>
    /// <seealso cref="IoC.Configuration.ISettingsRequestor" />
    public class SettingsRequestorDefault : ISettingsRequestor
    {
        #region Member Variables

        #endregion

        #region ISettingsRequestor Interface Implementation

        /// <summary>
        ///     Gets the collection of settings, that should be present in configuration file.
        /// </summary>
        /// <value>
        ///     The required settings.
        /// </value>
        [NotNull]
        [ItemNotNull]
        public IEnumerable<SettingInfo> RequiredSettings { get; } = new List<SettingInfo>();

        #endregion
    }
}