using SharedServices.Implementations;
using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.ValueImplementation.Services
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly string _serverName;
        private readonly string _databaseName;
        private readonly string _userName;
        private readonly string _password;

        public DbConnectionProvider(string serverName, string databaseName, string userName, string password)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            _userName = userName;
            _password = password;
        }


        public IDbConnection GetDbConnection()
        {
            return new SqlServerDbConnection(_serverName, _databaseName, _userName, _password);
        }
    }
}