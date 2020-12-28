using System.Data;
using System.Linq;
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
            await PairPlayers(team);
        }

        public void Delete(Team team)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Team> GetById(int id)
        {
            var teams = await connection.QueryAsync<Team>(
                @"SELECT * FROM Teams WHERE Id = @Id",
                new { Id = id }
            );
            return teams.FirstOrDefault();
        }
        public void Update(Team team)
        {
            throw new System.NotImplementedException();
        }
        public async Task PairPlayers(Team team)
        {
            var playerQuery = await connection.QueryAsync<int>(
                @"SELECT Id FROM Teams WHERE Name = @Name", 
                new { Name = team.Name }
            );
            int teamId = playerQuery.FirstOrDefault();
            foreach (Player player in team.Players)
            {
                await connection.ExecuteAsync(
                    @"UPDATE Players SET TeamId = @TeamId WHERE Id = @PlayerId", 
                    new {TeamId = teamId, PlayerId = player.Id});
            }
        }
    }
}