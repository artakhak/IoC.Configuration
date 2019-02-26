namespace TestPluginAssembly1.Interfaces
{
    public interface IDemoProxyService_Extension : IDemoProxyService, IDemoProxyService2
    {
        string GetTextValue();
    }
}