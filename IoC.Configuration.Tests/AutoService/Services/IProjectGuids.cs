using System;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public interface IProjectGuids
    {
        Guid Project1 { get; set; }
        Guid Project2 { get; }

        Guid GetDefaultProject();

        void NotImplementedReturnedValueVoid(int param1, out int param2, ref string param3);
        int NotImplementedReturnedValueInt(int param1, out int param2, ref string param3);
        int NotImplementedProperty { get; set; }
    }
}