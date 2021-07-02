using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Repositories;
using CribblyBackend.Core.Teams.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Players.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IDbConnection connection;
        public PlayerRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<bool> Exists(string email)
        {
            return (await connection.QueryAsync<bool>(
                PlayerQueries.PlayerExistsWithEmail,
                new { Email = email }
            )).Single();
        }

        public async Task<Player> Create(string email, string name)
        {
            await connection.ExecuteAsync(
                PlayerQueries.CreatePlayerQuery,
                new { Email = email, Name = name }
            );
            return await GetByEmail(email);
        }

        public void Delete(Player player)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Player> GetById(int id)
        {
            var players = await connection.QueryAsync<Player, Team, Player>(
                PlayerQueries.GetById,
                MapTeamToPlayer,
                new { Id = id }
            );
            return players.FirstOrDefault();
        }
        public async Task<Player> GetByEmail(string email)
        {
            var players = await connection.QueryAsync<Player, Team, Player>(
                PlayerQueries.GetByEmail,
                MapTeamToPlayer,
                new { Email = email }
            );
            return players.FirstOrDefault();
        }

        public void Update(Player player)
        {
            throw new System.NotImplementedException();
        }

        private Player MapTeamToPlayer(Player player, Team team)
        {
            if (team == null || team.Id == 0)
            {
                player.Team = null;
                return player;
            }
            player.Team = team;
            return player;
        }
    }
}