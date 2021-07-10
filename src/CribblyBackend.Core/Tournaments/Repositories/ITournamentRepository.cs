using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Models;

namespace CribblyBackend.Core.Tournaments.Repositories
{
    public interface ITournamentRepository
    {
        Task<Tournament> Create(DateTime date);
        Task SetFlagValue(int tournamentId, string flagName, bool newVal);
        Task<IEnumerable<Tournament>> GetTournamentsWithActiveFlag(string flagName);
    }
}