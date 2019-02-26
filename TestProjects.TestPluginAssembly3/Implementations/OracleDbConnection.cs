namespace TestPluginAssembly3.Implementations
{
    public class OracleDbConnection : SharedServices.Interfaces.IDbConnection
    {
        public OracleDbConnection(string connectionString)
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
