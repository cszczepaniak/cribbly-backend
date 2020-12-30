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
            List<Player> playerList = new List<Player>();
            //This returns two Team records, with each player showing up as the sole element of each Teams' Player List<Player>
            var teamRecords = await connection.QueryAsync<Team, Player, Team>(
                @"SELECT t.*, p.* FROM Teams t INNER JOIN Players p ON t.Id = p.TeamId WHERE p.TeamId = @Id", 
                MapPlayerToTeams, 
                new {Id = id}
            );
            //Add the Player List<Player> property from every record into a blank list
            foreach (Team team in teamRecords)
            {
                playerList.AddRange(team.Players);
            }
            /*
                We only need one copy of the Team object properties, so get the FirstOrDefault 
                and overwrite the Player property with what we grabbed before
            */
            Team teamToReturn = teamRecords.FirstOrDefault();
            teamToReturn.Players = playerList;

            return teamToReturn;
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
                lookup.Add(team.Id, teamToReturn = team);
            }
            if (teamToReturn.Players == null) 
            {
                teamToReturn.Players = new List<Player>();
            }

            teamToReturn.Players.Add(player);
            return teamToReturn;
        }
    }
}