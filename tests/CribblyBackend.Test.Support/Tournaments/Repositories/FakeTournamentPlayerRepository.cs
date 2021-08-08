using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Repositories;

namespace CribblyBackend.Test.Support.Tournaments.Repositories
{
    public class FakeTournamentPlayerRepository : ITournamentPlayerRepository
    {
        private readonly Dictionary<int, HashSet<int>> _tournamentPlayerLookup;
        public FakeTournamentPlayerRepository()
        {
            _tournamentPlayerLookup = new();
        }
        public Task CreateAsync(int tournamentId, int playerId)
        {
            if (!_tournamentPlayerLookup.ContainsKey(tournamentId))
            {
                _tournamentPlayerLookup[tournamentId] = new();
            }
            _tournamentPlayerLookup[tournamentId].Add(playerId);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int tournamentId, int playerId)
        {
            _tournamentPlayerLookup[tournamentId].Remove(playerId);
            return Task.CompletedTask;
        }

        public bool HasAssociation(int tournamentId, int playerId)
        {
            if (!_tournamentPlayerLookup.ContainsKey(tournamentId))
            {
                return false;
            }
            return _tournamentPlayerLookup[tournamentId].Contains(playerId);
        }
    }
}