using System;
using System.Collections.Generic;
using System.Text;

namespace IoC.Configuration.Tests.ProxyService.Services
{
    public interface IInterface1_Extension : IInterface1
    {
        string Text { get; }
    }
}
