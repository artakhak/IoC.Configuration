using System;
using System.Collections.Generic;
using System.Text;

namespace IoC.Configuration.Tests.ClassMember.Services
{
    public interface IAppInfoFactory
    {
        IAppInfo CreateAppInfo(int appId, string appDescription);
    }
}
