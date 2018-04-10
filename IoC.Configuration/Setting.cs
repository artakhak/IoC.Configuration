using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public class Setting : NamedValue, ISetting
    {
        #region Member Variables

        [NotNull]
        private readonly ISettingElement _settingElement;

        #endregion

        #region  Constructors

        public Setting([NotNull] ISettingElement settingElement) : base(settingElement)
        {
            _settingElement = settingElement;
        }

        #endregion

        #region ISetting Interface Implementation

        public object DeserializedValue => _settingElement.DeserializedValue;

        #endregion
    }
}