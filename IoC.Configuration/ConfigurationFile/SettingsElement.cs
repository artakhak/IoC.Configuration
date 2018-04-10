using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class SettingsElement : ConfigurationFileElementAbstr, ISettingsElement
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<string, ISettingElement> _settingNameToSettingMap = new Dictionary<string, ISettingElement>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region  Constructors

        public SettingsElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region ISettingsElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is ISettingElement)
            {
                var setting = (ISettingElement) child;

                if (_settingNameToSettingMap.ContainsKey(setting.Name))
                    throw new ConfigurationParseException(child, $"Multiple occurrences of setting with name '{setting.Name}'.", this);

                _settingNameToSettingMap[setting.Name] = setting;
            }
        }

        public IEnumerable<ISettingElement> AllSettings => _settingNameToSettingMap.Values.Where(x => x.Enabled);

        #endregion
    }
}