using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Divisions.Repositories;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Test.Support.Common;

namespace CribblyBackend.Test.Support.Divisions.Repositories
{
    public class FakeDivisionRepository : FakeRepository, IDivisionRepository
    {
        private readonly Dictionary<int, Division> _divisionsById;
        public Task<Division> AddTeamAsync(int id, Team team)
        {
            if (!_divisionsById.ContainsKey(id))
            {
                throw new Exception("Division not found");
            }
            var div = _divisionsById[id];
            if (div.Teams == null)
            {
                div.Teams = new();
            }
            div.Teams.Add(team);
            return Task.FromResult(div);
        }

        public Task<Division> CreateAsync(Division division)
        {
            division.Id = IncrementId();
            _divisionsById[division.Id] = division;
            return Task.FromResult(division);
        }

        public Task<Division> GetByIdAsync(int id)
        {
            if (_divisionsById.TryGetValue(id, out var division))
            {
                return Task.FromResult(division);
            }
            return Task.FromResult<Division>(null);
        }

        public Task DeleteAsync(Division division)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Division diivision)
        {
            throw new NotImplementedException();
        }
    }
}