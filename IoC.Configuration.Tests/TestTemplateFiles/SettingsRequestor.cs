using System.Collections.Generic;

namespace IoC.Configuration.Tests.TestTemplateFiles
{
    public class SettingsRequestor : ISettingsRequestor
    {
        public SettingsRequestor()
        {
            var requiredSettings = new List<SettingInfo>();
            requiredSettings.Add(new SettingInfo("int32Setting", typeof(int)));
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
