using System;
using System.Collections.Generic;
using System.Text;

namespace IoC.Configuration.Tests.ProxyService.Services
{
    public class Interface1User
    {
        public Interface1User(IInterface1 param1)
        {
            Interface1Property = param1;
        }

        public IInterface1 Interface1Property { get; }
    }
}
