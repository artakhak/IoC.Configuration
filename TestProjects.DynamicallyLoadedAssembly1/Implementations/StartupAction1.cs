using System;
using IoC.Configuration;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;
using SharedServices.Implementations;

namespace DynamicallyLoadedAssembly1.Implementations
{
    public class StartupAction1 : StartupActionBaseForTests
    {
        #region  Constructors

        public StartupAction1([NotNull] ISettings settings)
        {
            if (!settings.GetSettingValue("SynchronizerFrequencyInMilliseconds", 0, out var settingValue))
                throw new Exception("Setting not found");

            LogHelper.Context.Log.InfoFormat("The value of settings 'SynchronizerFrequencyInMilliseconds' is '{0}'.",
                settings.GetSettingValueOrThrow<int>("SynchronizerFrequencyInMilliseconds"));
        }

        #endregion
    }
}