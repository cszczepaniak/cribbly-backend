using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Teams.Repositories;
using CribblyBackend.DataAccess.Exceptions;

namespace CribblyBackend.Test.Support.Teams.Repositories
{
    public class FakeTeamRepository : ITeamRepository
    {
        private int nextId;
        private readonly Dictionary<int, Team> _teamsById;
        private readonly HashSet<string> _teamNames;

        public FakeTeamRepository()
        {
            _teamNames = new();
            _teamsById = new();
            nextId = 0;
        }
        public Task<int> CreateAsync(Team team)
        {

            if (team.Players == null)
            {
                throw new Exception("Must set Players when creating a team");
            }
            if (team.Players.Count < 2)
            {
                throw new Exception("A Team must not have less than two players");
            }
            if (_teamNames.Contains(team.Name))
            {
                throw new Exception("Duplicate team name");
            }
            Interlocked.Increment(ref nextId);
            team.Id = nextId;
            _teamsById[team.Id] = team;
            _teamNames.Add(team.Name);
            return Task.FromResult(team.Id);
        }

        public async Task DeleteAsync(int id)
        {
            if (await GetByIdAsync(id) == null)
            {
                throw new TeamNotFoundException(id);
            };
            _teamsById.Remove(id);
        }

        public Task<List<Team>> GetAllAsync()
        {
            return Task.FromResult(_teamsById.Values.ToList());
        }

        public Task<Team> GetByIdAsync(int id)
        {
            if (_teamsById.TryGetValue(id, out var team))
            {
                return Task.FromResult(team);
            }
            return Task.FromResult<Team>(null);
        }

        public Task UpdateAsync(Team team)
        {
            throw new NotImplementedException();
        }
    }
}