using System.Collections.Generic;

namespace IoC.Configuration.Tests.GenericTypesAndTypeReUse
{
    public class SettingsRequestor : ISettingsRequestor
    {
        public SettingsRequestor()
        {
            
        }

        /// <summary>
        /// Gets the collection of settings, that should be present in configuration file.
        /// </summary>
        /// <value>
        /// The required settings.
        /// </value>
        public IEnumerable<SettingInfo> RequiredSettings { get; }

    }
}
