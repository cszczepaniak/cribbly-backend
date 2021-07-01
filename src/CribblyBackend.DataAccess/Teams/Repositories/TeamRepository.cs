using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Teams;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Teams.Repositories;
using CribblyBackend.DataAccess.Extensions;
using Dapper;

namespace CribblyBackend.DataAccess.Teams.Repositories
{
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
            var teams = (await _connection.QueryWithObjectAsync<Team, Player, Team>(
                TeamQueries.GetAll(),
                (t, p) =>
                {
                    p.Team = new Team() { Id = t.Id };
                    players.Add(p);
                    return t;
                }
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
            await _connection.ExecuteWithObjectAsync(TeamQueries.CreateWithName(team.Name));
            foreach (Player player in team.Players)
            {
                await _connection.ExecuteWithObjectAsync(TeamQueries.UpdatePlayerWithLastTeamId(player.Id));
            }
            return await _connection.QueryLastInsertedId();
        }

        public void Delete(Team team)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Team> GetById(int id)
        {
            var players = new Dictionary<int, Player>();
            var teams = await _connection.QueryAsync<Team, Player, Division, Team>(
                $@"
                SELECT 
                    t.Id, t.Name, 
                    p.Id, p.Email, p.Name, p.Role, p.TeamId, 
                    d.Id, d.Name
                FROM Teams t 
                LEFT JOIN Players p 
                ON t.Id = p.TeamId
                LEFT JOIN Divisions d
                ON t.Division = d.Id 
                WHERE t.Id = @Id",
                (t, p, d) =>
                {
                    t.Division = d;
                    if (p == null)
                    {
                        return t;
                    }
                    if (!players.TryGetValue(p.Id, out Player _))
                    {
                        players.Add(p.Id, p);
                    }
                    return t;
                },
                new { Id = id }
            );
            var team = teams.Single();
            team.Players = players.Values.ToList();
            return team;
        }
        public void Update(Team team)
        {
            throw new System.NotImplementedException();
        }
    }
}