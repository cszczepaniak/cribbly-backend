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
        Task<List<Team>> GetAll();
        void Update(Team Team);
        Task<int> Create(Team Team);
        void Delete(Team Team);
    }
    public class TeamRepository : ITeamRepository
    {
        private readonly IDbConnection _connection;
        public TeamRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<List<Team>> GetAll()
        {
            var players = new List<Player>();
            var teams = (await _connection.QueryAsync<Team, Player, Team>(
                @"SELECT * FROM Teams t INNER JOIN Players p ON t.Id = p.TeamId",
                (t, p) => 
                {
                    switch (players.Count)
                    {
                        case 0:
                            players.Add(p);
                            break;
                        case 1:
                            players.Add(p);
                            t.Players = new List<Player>();
                            t.Players.AddRange(players);
                            players.Clear();
                            break;
                    }

                    return t;
                },
                splitOn: "Id"
                )).Where(t => t.Players != null).ToList();
            return teams;
        }
        public async Task<int> Create(Team team)
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
            return (await _connection.QueryAsync<int>(@"SELECT LAST_INSERT_ID()")).First();
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