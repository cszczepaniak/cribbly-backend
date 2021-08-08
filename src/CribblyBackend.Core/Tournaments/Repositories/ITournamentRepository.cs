using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Models;

namespace CribblyBackend.Core.Tournaments.Repositories
{
    public interface ITournamentRepository
    {
        Task<Tournament> CreateAsync(DateTime date);
        Task SetFlagValueAsync(int tournamentId, string flagName, bool newVal);
        Task<IEnumerable<Tournament>> GetTournamentsWithActiveFlagAsync(string flagName);
        Task<Tournament> GetByIdAsync(int id);
    }
}