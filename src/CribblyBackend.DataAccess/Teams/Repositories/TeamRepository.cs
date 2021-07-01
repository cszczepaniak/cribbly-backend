using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Teams;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Teams.Repositories;
using CribblyBackend.DataAccess.Common;
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
            var playerMap = new Dictionary<int, List<Player>>();
            var teams = await _connection.QueryAsync<Team, Player, Team>(
                TeamQueries.GetAll,
                (t, p) =>
                {
                    if (playerMap.TryGetValue(t.Id, out var ps))
                    {
                        ps.Add(p);
                        return t;
                    }
                    playerMap[t.Id] = new List<Player> { p };
                    return t;
                }
            );
            foreach (var team in teams)
            {
                team.Players = playerMap[team.Id];
            }
            return teams.ToList();
        }
        public async Task<int> Create(Team team)
        {
            if (team.Players.Count < 2)
            {
                throw new System.Exception("A Team must not have less than two players");
            }
            await _connection.ExecuteAsync(
                TeamQueries.CreateWithName,
                Query.Params("@Name", team.Name)
            );
            var updateTasks = new List<Task>(team.Players.Count);
            foreach (Player player in team.Players)
            {
                updateTasks.Add(
                    _connection.ExecuteAsync(
                        TeamQueries.UpdatePlayerWithLastTeamId,
                        Query.Params("@Id", player.Id)
                    )
                );
            }
            await Task.WhenAll(updateTasks);
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
                TeamQueries.GetById,
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
                Query.Params("@Id", id)
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