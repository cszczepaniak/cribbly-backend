using System;

namespace CribblyBackend.Common
{
    public class Config
    {
        internal static string FirebaseAudience => Environment.GetEnvironmentVariable("FIREBASE_PROJ_ID");
        internal static DbConfig MySqlConfig => new DbConfig(
            port: Environment.GetEnvironmentVariable("DB_PORT"),
            host: Environment.GetEnvironmentVariable("DB_HOST"),
            user: Environment.GetEnvironmentVariable("DB_USER"),
            password: Environment.GetEnvironmentVariable("DB_PASSWORD"),
            dbName: Environment.GetEnvironmentVariable("DB_NAME")
        );
    }

    internal class DbConfig
    {
        private readonly string _port;
        private readonly string _host;
        private readonly string _user;
        private readonly string _password;
        private readonly string _dbName;
        public DbConfig(string port, string host, string user, string password, string dbName)
        {
            _port = port;
            _host = host;
            _user = user;
            _password = password;
            _dbName = dbName;
        }
        internal string Connection => $"server={_host};port={_port};database={_dbName};uid={_user};pwd={_password};";
    }

}