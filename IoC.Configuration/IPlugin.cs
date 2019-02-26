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

namespace IoC.Configuration
{
    /// <summary>
    ///     Represents a plugin object in configuration file.
    /// </summary>
    /// <seealso cref="IoC.Configuration.ISettingsRequestor" />
    /// <seealso cref="System.IDisposable" />
    public interface IPlugin : ISettingsRequestor, IDisposable
    {
        #region Current Type Interface

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        ///     Sets/gets the <see cref="IPluginData" /> object corresponding to plugin, retrieved from configuration.
        ///     The implementation should ensure that the plugin data can be set only once, when the configuration is loaded.
        ///     The implementation can subclass from <see cref="PluginAbstr" /> to re-use this implementation.
        /// </summary>
        IPluginData PluginData { get; set; }

        #endregion
    }
}