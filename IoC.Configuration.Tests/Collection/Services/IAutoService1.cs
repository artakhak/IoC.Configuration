using System.Collections.Generic;

namespace IoC.Configuration.Tests.Collection.Services
{
    public interface IAutoService1
    {
        IReadOnlyList<int> GetAllActionIds(int appId);
        IReadOnlyList<string> Texts { get; }
    }
}
