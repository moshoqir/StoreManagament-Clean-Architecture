using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Data
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        public string ConnectionString { get; }

        // Constructor to inject the connection string and validate that it isn't empty.
        // Note: This validates the string format, but does not open the DB connection yet.
        public SqlConnectionFactory(string _connectionString)
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentException("Database connection string can't be empty.",
                    nameof(_connectionString));
            }

            ConnectionString = _connectionString;
        }

        // Creates and returns a new SqlConnection object using the stored connection string.
        // Dapper (or the repository) will be responsible for actually opening this connection.
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
