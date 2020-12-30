using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Models;
using Dapper;

namespace CribblyBackend.Services
{
    public interface IPlayerService
    {
        Task<Player> GetByEmail(string email);
        Task<Player> GetById(int id);
        void Update(Player player);
        Task Create(Player player);
        void Delete(Player player);
    }
    public class PlayerService : IPlayerService
    {
        private readonly IDbConnection connection;
        public PlayerService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task Create(Player player)
        {
            await connection.ExecuteAsync(
                @"INSERT INTO Players (Email, Name, TeamId, Role)
                VALUES (@Email, @Name, @TeamId, @Role)",
                new { Email = player.Email, Name = player.Name, TeamId = player.Team?.Id, Role = player.Role }
            );
        }

        public void Delete(Player player)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Player> GetById(int id)
        {
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
            var players = await connection.QueryAsync<Player, Team, Player>(
                @"SELECT * FROM Players p LEFT JOIN Teams t ON p.TeamId = t.Id WHERE p.Email = @Email",
                MapTeamToPlayer,
                new { Email = email },
                splitOn: "TeamId"
            );
            return players.FirstOrDefault();
        }

        public void Update(Player player)
        {
            throw new System.NotImplementedException();
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