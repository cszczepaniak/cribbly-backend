using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Exceptions;
using CribblyBackend.DataAccess.Services;
using Dapper;

namespace CribblyBackend.DataAccess.Repositories
{
    public interface ITeamRepository
    {
        Task<Team> GetById(int Id);
        Task<List<Team>> Get();
        void Update(Team Team);
        Task<int> Create(Team Team);
        Task Delete(int id);
    }
    public class TeamRepository : ITeamRepository
    {
        private readonly IDbConnection _connection;
        public TeamRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<List<Team>> Get()
        {
            var players = new List<Player>();
            var teams = (await _connection.QueryAsync<Team, Player, Team>(
                @"SELECT * FROM Teams t 
                INNER JOIN Players p ON t.Id = p.TeamId",
                (t, p) => 
                {
                    p.Team = new Team(){ Id = t.Id };
                    players.Add(p);
                    return t;
                },
                splitOn: "Id"
                )).ToList();
                foreach (Team team in teams)
                {
                    var members = players.Where(p => p.Team.Id == team.Id).ToList();
                    team.Players = members;
                }
            return teams.Distinct(new TeamComparer()).ToList();
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

        public async Task Delete(int id)
        {
            if (this.GetById(id).Result == null)
            {
                throw new TeamNotFoundException();
            };

            await _connection.ExecuteAsync(
                @"
                    UPDATE players
                    SET players.TeamId = null
                    WHERE TeamId = @Id;

                    DELETE FROM scores
                    WHERE TeamId = @Id;

                    UPDATE games
                    SET games.WinnerId = null
                    WHERE WinnerId = @Id;

                    DELETE FROM teams
                    WHERE teams.Id = @Id;
                ",
                new { Id = id }
            );
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

            if (team != null)
            {
                team.Players = players.Values.ToList();
            }

            return team;
        }
        public void Update(Team team)
        {
            throw new System.NotImplementedException();
        }
    }
}