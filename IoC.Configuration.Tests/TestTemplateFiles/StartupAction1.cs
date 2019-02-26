using System;
using System.Collections.Generic;
using System.Text;
using OROptimizer.Diagnostics.Log;
using SharedServices.Implementations;

namespace IoC.Configuration.Tests.TestTemplateFiles
{
    public class StartupAction1 : StartupActionBaseForTests
    {
        public override void Start()
        {
            base.Start();

            LogHelper.Context.Log.Info($"{typeof(StartupAction1)}.{nameof(Start)}() called.");
        }

        public override void Stop()
        {
            base.Stop();
            LogHelper.Context.Log.Info($"{typeof(StartupAction1)}.{nameof(Stop)}() called.");
        }
    }
}
