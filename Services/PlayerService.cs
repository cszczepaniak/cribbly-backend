using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Models;
using Dapper;

namespace CribblyBackend.Services
{
    public interface IPlayerService
    {
        void Initialize();
        Task<Player> GetByEmail(string email);
        void Update(Player player);
        Task Create(Player player);
        void Delete(Player player);
    }
    public class PlayerService : IPlayerService
    {
        IDbConnection connection;
        public PlayerService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task Create(Player player)
        {
            await connection.ExecuteAsync(
                @"INSERT INTO Players (Email, Name, Team, Role)
                VALUES (@Email, @Name, @Team, @Role)",
                new { Email = player.Email, Name = player.Name, Team = player.Team?.Id, Role = player.Role }
            );
        }

        public void Delete(Player player)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Player> GetByEmail(string email)
        {
            var players = await connection.QueryAsync<Player>(
                @"SELECT * FROM Players WHERE Email = @Email",
                new { Email = email }
            );
            return players.FirstOrDefault();
        }

        public void Initialize()
        {
            connection.Execute(
                @"CREATE TABLE IF NOT EXISTS Players (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Email VARCHAR(100) NOT NULL,
                    Name VARCHAR(100) NOT NULL,
                    Team INT,
                    Role VARCHAR(100)
                );"
            );
        }

        public void Update(Player player)
        {
            throw new System.NotImplementedException();
        }
    }
}