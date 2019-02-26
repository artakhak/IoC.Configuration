using System.Collections.Generic;

namespace IoC.Configuration.Tests.ClassMember.Services
{
    public interface IAppInfos
    {
        IReadOnlyList<IAppInfo> AllAppInfos { get; }
    }
}
