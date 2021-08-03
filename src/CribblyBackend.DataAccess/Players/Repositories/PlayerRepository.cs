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
        private readonly IDbConnection _connection;
        public PlayerRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> ExistsAsync(string authProviderId)
        {
            return (await _connection.QueryAsync<bool>(
                PlayerQueries.PlayerExistsWithAuthProviderId,
                new { AuthProviderId = authProviderId }
            )).Single();
        }

        public async Task<Player> CreateAsync(Player player)
        {
            await _connection.ExecuteAsync(
                PlayerQueries.CreatePlayerQuery,
                player
            );
            return await GetByAuthProviderIdAsync(player.AuthProviderId);
        }

        public void Delete(Player player)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Player> GetByIdAsync(int id) =>
            await GetPlayerAsync(PlayerQueries.GetById, new { Id = id });
        public async Task<Player> GetByAuthProviderIdAsync(string authProviderId) =>
            await GetPlayerAsync(PlayerQueries.GetByAuthProviderId, new { AuthProviderId = authProviderId });
        public async Task<Player> GetByEmailAsync(string email) =>
            await GetPlayerAsync(PlayerQueries.GetByEmail, new { Email = email });

        private async Task<Player> GetPlayerAsync(string query, object queryParams)
        {
            var players = await _connection.QueryAsync<Player, Team, Player>(
                query,
                (p, t) =>
                {
                    if (t == null || t.Id == 0)
                    {
                        p.Team = null;
                        return p;
                    }
                    p.Team = t;
                    return p;
                },
                queryParams
            );
            return players.SingleOrDefault();
        }

        public void Update(Player player)
        {
            throw new System.NotImplementedException();
        }
    }
}