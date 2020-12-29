using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Models;
using Dapper;
using Newtonsoft.Json;

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
            await connection.ExecuteAsync(
                @"INSERT INTO Teams(Name) VALUES (@Name)", 
                new { Name = team.Name }
            );
            foreach (Player player in team.Players)
            {
                await connection.ExecuteAsync(
                    @"UPDATE Players SET TeamId = LAST_INSERT_ID() WHERE Id = @PlayerId", 
                    new {PlayerId = player.Id});
            }
        }

        public void Delete(Team team)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Team> GetById(int id)
        {
            var teams = await connection.QueryAsync<Team, List<Player>, Team>(
                @"SELECT * FROM Teams t LEFT JOIN Players p ON t.Id = p.TeamId WHERE t.Id = @Id",
                MapPlayersToTeams,
                new { Id = id }, 
                splitOn: "Id"
            );
            return teams.FirstOrDefault();
        }
        public void Update(Team team)
        {
            throw new System.NotImplementedException();
        }

        private Team MapPlayersToTeams(Team team, List<Player> players)
        {
            team.Players = players;
            foreach(Player player in players)
            {
                team.Players.Add(player);
            }
            return team;
        }
    }
}