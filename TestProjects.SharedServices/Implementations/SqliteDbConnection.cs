using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class SqliteDbConnection : IDbConnection
    {

        public SqliteDbConnection(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }

        /// <summary>
        /// Opens a connection to database.
        /// </summary>
        /// <exception cref="System.Exception">Thross an exception if connection cannot be opened.</exception>
        public void Open()
        {
            // Open a connection to Sqlite database using _filePath
        }

        public string ConnectionString => FilePath;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            // Some implementation here to close the connection, or to return it to database connection pool.
        }
    }
}