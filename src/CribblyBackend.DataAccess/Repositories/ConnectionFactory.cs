using System.Data;
using MySql.Data.MySqlClient;

namespace CribblyBackend.DataAccess.Repositories
{
    public interface IConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }

    public class ConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetOpenConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}