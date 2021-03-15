using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Repositories
{
    public interface IPlayerRepository
    {
        Task<bool> Exists(string email);
        Task<Player> GetByEmail(string email);
        Task<Player> GetById(int id);
        Task<Player> Create(string email, string name);
    }
    public class PlayerRepository : RepositoryBase, IPlayerRepository
    {
        public PlayerRepository(IConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        public async Task<bool> Exists(string email)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            return (await connection.QueryAsync<bool>(
                @"SELECT EXISTS(SELECT * FROM Players WHERE Email = @Email LIMIT 1)",
                new { Email = email }
            )).First();
        }

        public async Task<Player> Create(string email, string name)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            await connection.ExecuteAsync(
                @"INSERT INTO Players (Email, Name) VALUES (@Email, @Name)",
                new { Email = email, Name = name }
            );
            return await GetByEmail(email);
        }

        public async Task<Player> GetById(int id)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            var players = await connection.QueryAsync<Player, Team, Player>(
                @"SELECT * FROM Players p LEFT JOIN Teams t ON p.TeamId = t.Id WHERE p.Id = @Id",
                MapTeamToPlayer,
                new { Id = id },
                splitOn: "TeamId"
            );
            return players.FirstOrDefault();
        }
        public async Task<Player> GetByEmail(string email)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            var players = await connection.QueryAsync<Player, Team, Player>(
                @"SELECT * FROM Players p LEFT JOIN Teams t ON p.TeamId = t.Id WHERE p.Email = @Email",
                MapTeamToPlayer,
                new { Email = email },
                splitOn: "TeamId"
            );
            return players.FirstOrDefault();
        }

        private Player MapTeamToPlayer(Player player, Team team)
        {
            if (team.Id == 0)
            {
                player.Team = null;
                return player;
            }
            player.Team = team;
            return player;
        }
    }
}