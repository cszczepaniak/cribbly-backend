using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Models;
using CribblyBackend.Core.Tournaments.Repositories;
using CribblyBackend.Test.Support.Common;

namespace CribblyBackend.Test.Support.Tournaments.Repositories
{
    public class FakeTournamentRepository : FakeRepository, ITournamentRepository
    {
        private readonly Dictionary<int, Tournament> _tournamentsById;
        private readonly Dictionary<string, Dictionary<int, Tournament>> _tournamentsByActiveFlag;
        public FakeTournamentRepository()
        {
            _tournamentsById = new();
            _tournamentsByActiveFlag = new();
            _tournamentsByActiveFlag[nameof(Tournament.IsActive)] = new();
            _tournamentsByActiveFlag[nameof(Tournament.IsOpenForRegistration)] = new();
        }
        public Task<Tournament> CreateAsync(DateTime date)
        {
            var id = IncrementId();
            _tournamentsById[id] = new() { Id = id, Date = date };
            return Task.FromResult(_tournamentsById[id]);
        }

        public Task<Tournament> GetByIdAsync(int id)
        {
            if (_tournamentsById.TryGetValue(id, out var tournament))
            {
                return Task.FromResult(tournament);
            }
            throw new Exception("Tournament not found");
        }

        public Task<IEnumerable<Tournament>> GetTournamentsWithActiveFlagAsync(string flagName)
        {
            return Task.FromResult<IEnumerable<Tournament>>(_tournamentsByActiveFlag[flagName].Values);
        }

        public Task SetFlagValueAsync(int tournamentId, string flagName, bool newVal)
        {
            if (!_tournamentsById.ContainsKey(tournamentId))
            {
                return Task.CompletedTask;
            }
            switch (flagName)
            {
                case nameof(Tournament.IsActive):
                    _tournamentsById[tournamentId].IsActive = newVal;
                    break;
                case nameof(Tournament.IsOpenForRegistration):
                    _tournamentsById[tournamentId].IsOpenForRegistration = newVal;
                    break;
            }
            if (newVal)
            {
                _tournamentsByActiveFlag[flagName][tournamentId] = _tournamentsById[tournamentId];
            }
            else
            {
                _tournamentsByActiveFlag[flagName].Remove(tournamentId);
            }
            return Task.CompletedTask;
        }
    }
}