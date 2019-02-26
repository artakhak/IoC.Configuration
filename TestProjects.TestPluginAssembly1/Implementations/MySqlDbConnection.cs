namespace TestPluginAssembly1.Implementations
{
    public class MySqlDbConnection : SharedServices.Interfaces.IDbConnection
    {
        public MySqlDbConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Opens a connection to database.
        /// </summary>
        /// <exception cref="System.Exception">Throws an exception if connection cannot be opened.</exception>
        public void Open()
        {

        }
    }
}
