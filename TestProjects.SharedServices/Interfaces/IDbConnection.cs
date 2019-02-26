using System;

namespace SharedServices.Interfaces
{
    public interface IDbConnection : IDisposable
    {
        /// <summary>
        /// Opens a connection to database.
        /// </summary>
        /// <exception cref="System.Exception">Throws an exception if connection cannot be opened.</exception>
        void Open();
        // Some other methods to get reader, etc...

        string ConnectionString { get; }
    }
}