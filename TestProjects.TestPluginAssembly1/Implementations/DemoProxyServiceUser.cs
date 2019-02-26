using TestPluginAssembly1.Interfaces;

namespace TestPluginAssembly1.Implementations
{
    public class DemoProxyServiceUser
    {
        public DemoProxyServiceUser(IDemoProxyService demoProxyService)
        {
            DemoProxyService = demoProxyService;
        }

        public IDemoProxyService DemoProxyService { get; }
    }
}