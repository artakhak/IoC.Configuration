namespace IoC.Configuration.ConfigurationFile
{
    public class SettingValueInitializerHelper : ISettingValueInitializerHelper
    {
        public ISettingElement GetSettingElement(IConfigurationFileElement requestingConfigurationFileElement, string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
                throw new ConfigurationParseException(requestingConfigurationFileElement, "The setting name cannot be empty.");

            ISettingElement _settingElement = null;
            if (requestingConfigurationFileElement.OwningPluginElement != null)
                _settingElement = requestingConfigurationFileElement.GetPluginSetupElement().SettingsElement?.GetSettingElement(settingName);

            if (_settingElement == null)
                _settingElement = requestingConfigurationFileElement.Configuration.SettingsElement?.GetSettingElement(settingName);

            if (_settingElement == null)
                throw new ConfigurationParseException(requestingConfigurationFileElement, $"Setting with name '{settingName}' was not found.");

            return _settingElement;
        }
    }
}