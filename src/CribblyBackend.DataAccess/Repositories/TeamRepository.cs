using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Repositories
{
    public interface ITeamRepository
    {
        Task<Team> GetById(int Id);
        void Update(Team Team);
        Task Create(Team Team);
        void Delete(Team Team);
    }
    public class TeamRepository : ITeamRepository
    {
        private readonly IDbConnection _connection;
        public TeamRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task Create(Team team)
        {
            if (team.Players.Count < 2)
            {
                throw new System.Exception("A Team must not have less than two players");
            }
            await _connection.ExecuteAsync(
                @"INSERT INTO Teams(Name) VALUES (@Name)",
                new { Name = team.Name }
            );
            foreach (Player player in team.Players)
            {
                await _connection.ExecuteAsync(
                    @"UPDATE Players SET TeamId = LAST_INSERT_ID() WHERE Id = @PlayerId",
                    new { PlayerId = player.Id });
            }
        }

        public void Delete(Team team)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Team> GetById(int id)
        {
            var players = new Dictionary<int, Player>();
            var team = (await _connection.QueryAsync<Team, Player, Team>(
                @"SELECT t.*, p.* FROM Teams t INNER JOIN Players p ON t.Id = p.TeamId WHERE p.TeamId = @Id",
                (t, p) =>
                {
                    if (!players.TryGetValue(p.Id, out Player _))
                    {
                        players.Add(p.Id, p);
                    }
                    return t;
                },
                new { Id = id },
                splitOn: "Id"
                )).FirstOrDefault();
            team.Players = players.Values.ToList();
            return team;
        }
        public void Update(Team team)
        {
            throw new System.NotImplementedException();
        }
    }
}