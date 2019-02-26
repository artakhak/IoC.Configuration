using TestPluginAssembly1.Interfaces;

namespace TestPluginAssembly1.Implementations
{
    public class DemoProxyService_Extension_Impl : IDemoProxyService_Extension
    {
        public int GetIntValue() => 17;

        public string GetTextValue() => nameof(DemoProxyService_Extension_Impl);
        public int GetIntValue2()
        {
            return 3;
        }
    }
}