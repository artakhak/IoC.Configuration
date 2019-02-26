using System;
using System.Text;

namespace IoC.Configuration.Tests.ProxyService.Services
{
    public class AppData : IAppData
    {
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
    }
}
