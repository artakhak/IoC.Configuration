using System.Collections.Generic;

namespace IoC.Configuration.Tests.DocumentationTests
{
    public class Plugin1 : PluginAbstr
    {
        #region Member Variables

        private readonly List<SettingInfo> _requiredSettings;

        #endregion

        #region  Constructors

        public Plugin1(long param1)
        {
            Property1 = param1;
            _requiredSettings = new List<SettingInfo>();
            _requiredSettings.Add(new SettingInfo("Int32Setting1", typeof(int)));
            _requiredSettings.Add(new SettingInfo("StringSetting1", typeof(string)));
        }

        #endregion

        #region Member Functions

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            // Dispose resources
        }

        /// <summary>Initializes this instance.</summary>
        public override void Initialize()
        {
            // Do initialization here
        }

        public long Property1 { get; }
        public long Property2 { get; set; }

        #endregion
    }
}