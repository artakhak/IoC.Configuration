using System.Collections.Generic;
using System.Linq;
using IoC.Configuration.OnApplicationStart;

namespace IoC.Configuration.Tests.ConstructedValue.Services
{
    public class StartupActionsRetriever
    {
        public StartupActionsRetriever(IEnumerable<IStartupAction> startupActions)
        {
            StartupActions = startupActions.ToList();
        }

        public IReadOnlyList<IStartupAction> StartupActions { get; }
    }
}