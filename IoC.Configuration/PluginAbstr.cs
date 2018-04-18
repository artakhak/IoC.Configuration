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
using System.Linq;

namespace IoC.Configuration
{
    /// <summary>
    /// Base abstract implementation of <see cref="IPlugin"/>.
    /// Plugin implementation s specified in element iocConfiguration/pluginsSetup/pluginSetup/pluginImplementation can use this class
    /// as a parent class to use re-use some implementations of <see cref="IPlugin"/>. 
    /// </summary>
    /// <seealso cref="IoC.Configuration.IPlugin" />
    public abstract class PluginAbstr : IPlugin
    {
        #region Member Variables

        private IPluginData _pluginData;
        private bool _pluginDataWasSet;

        #endregion

        #region IPlugin Interface Implementation        
        /// <summary>
        /// Sets/gets the <see cref="IPluginData" /> object corresponding to plugin, retrieved from configuration.
        /// The implementation should ensure that the plugin data can be set only once, when the configuration is loaded.
        /// The implementation can subclass from <see cref="PluginAbstr" /> to re-use this implementation.
        /// </summary>
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
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Gets the collection of settings, that should be present in configuration file in element
        /// iocConfiguration/pluginsSetup/pluginSetup/settings.
        /// </summary>
        /// <value>
        /// The required settings.
        /// </value>
        public virtual IEnumerable<SettingInfo> RequiredSettings => Enumerable.Empty<SettingInfo>();

        #endregion
    }
}