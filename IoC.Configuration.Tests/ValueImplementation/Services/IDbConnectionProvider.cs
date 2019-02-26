using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.ValueImplementation.Services
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetDbConnection();
    }
}
