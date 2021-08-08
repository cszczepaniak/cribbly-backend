using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Models;
using CribblyBackend.Core.Tournaments.Repositories;

namespace CribblyBackend.Test.Support.Tournaments.Repositories
{
    public class FakeTournamentRepository : ITournamentRepository
    {
        public Task<Tournament> CreateAsync(DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<Tournament> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tournament>> GetTournamentsWithActiveFlagAsync(string flagName)
        {
            throw new NotImplementedException();
        }

        public Task SetFlagValueAsync(int tournamentId, string flagName, bool newVal)
        {
            throw new NotImplementedException();
        }
    }
}