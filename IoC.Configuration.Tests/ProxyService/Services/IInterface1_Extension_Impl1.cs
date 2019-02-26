namespace IoC.Configuration.Tests.ProxyService.Services
{
    public class Interface1_Extension_Impl1 : IInterface1_Extension
    {
        public int GetIntValue()
        {
            return 13;
        }

        public string Text { get; } = "Some text";
    }
}