using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Models;
using Dapper;

namespace CribblyBackend.Services
{
    public interface ITeamService
    {
        Task<Team> GetById(int Id);
        void Update(Team Team);
        Task Create(Team Team);
        void Delete(Team Team);
    }
    public class TeamService : ITeamService
    {
        IDbConnection connection;
        public TeamService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task Create(Team team)
        {
            if(team.Players.Count < 2)
            {
                throw new System.NullReferenceException("A Team must not have less than two players");
            }
            await connection.ExecuteAsync(
                @"INSERT INTO Teams(Name) VALUES (@Name)", 
                new { Name = team.Name }
            );
            foreach (Player player in team.Players)
            {
                await connection.ExecuteAsync(
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
            var team = (await connection.QueryAsync<Team, Player, Team>(
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

        private Team MapPlayerToTeams(Team team, Player player)
        {
            var lookup = new Dictionary<int, Team>();
            Team teamToReturn;
            if (!lookup.TryGetValue(team.Id, out teamToReturn)) 
            {
                teamToReturn = team;
                teamToReturn.Players = new List<Player>();
                lookup.Add(team.Id, team);
            }
            teamToReturn.Players.Add(player);
            return teamToReturn;
        }
    }
}