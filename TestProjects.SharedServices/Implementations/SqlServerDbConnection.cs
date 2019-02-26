using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class SqlServerDbConnection : IDbConnection
    {
        public SqlServerDbConnection(string serverName, string databaseName, string userName, string password)
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            UserName = userName;
            Password = password;

            ConnectionString = $"server={ServerName}, instance={DatabaseName}, username={UserName}, password={Password}";
        }
        /// <summary>
        /// Opens a connection to database.
        /// </summary>
        /// <exception cref="System.Exception">Throws an exception if connection cannot be opened.</exception>
        public void Open()
        {
            // Some implementation here to close the connection, or to return it to database connection pool.
        }

        public string ConnectionString { get; private set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            // Some implementation here to close the connection, or to return it to database connection pool.
        }

        public string ServerName { get; }
        public string DatabaseName { get; }
        public string UserName { get; }
        public string Password { get; }
    }
}