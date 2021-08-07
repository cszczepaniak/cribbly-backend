using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Teams.Repositories;
using CribblyBackend.DataAccess.Extensions;
using CribblyBackend.DataAccess.Exceptions;
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
        public async Task<List<Team>> GetAllAsync()
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
        public async Task<int> CreateAsync(Team team)
        {
            if (team.Players == null)
            {
                throw new System.Exception("Must set Players when creating a team");
            }
            if (team.Players.Count < 2)
            {
                throw new System.Exception("A Team must not have less than two players");
            }

            var createdId = 0;

            using (var scope = new TransactionScope())
            {
                await _connection.ExecuteAsync(
                    TeamQueries.CreateWithName,
                    new { Name = team.Name }
                );

                var createdIdTask = _connection.QueryLastInsertedId();
                var updatePlayerTask = _connection.ExecuteAsync(
                    TeamQueries.UpdatePlayerTeamToLastTeamId,
                    team.Players.Select(p => new { PlayerId = p.Id })
                );

                await Task.WhenAll(createdIdTask, updatePlayerTask);
                createdId = await createdIdTask;

                scope.Complete();
            }
            return createdId;
        }

        public async Task DeleteAsync(int id)
        {
            if (await GetByIdAsync(id) == null)
            {
                throw new TeamNotFoundException(id);
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

        public async Task<Team> GetByIdAsync(int id)
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
                new { Id = id }
            );
            var team = teams.FirstOrDefault();
            team.Players = players.Values.ToList();
            return team;
        }
        public Task UpdateAsync(Team team)
        {
            throw new System.NotImplementedException();
        }
    }
}