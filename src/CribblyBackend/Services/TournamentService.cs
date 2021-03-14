using System;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;

namespace CribblyBackend.Services
{
    public interface ITournamentService
    {
        Task<Tournament> GetNextTournament();
        Task<Tournament> Create(DateTime date);
        Task ChangeActiveStatus(int tournamentId, bool newVal);
        Task ChangeOpenForRegistrationStatus(int tournamentId, bool newVal);
    }
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;

        public TournamentService(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
        }

        public async Task<Tournament> Create(DateTime date)
        {
            return await _tournamentRepository.Create(date);
        }

        public async Task<Tournament> GetNextTournament()
        {
            return await _tournamentRepository.GetNextTournament();
        }

        public async Task ChangeActiveStatus(int tournamentId, bool newVal)
        {
            await _tournamentRepository.SetFlagValue(tournamentId, nameof(Tournament.IsActive), newVal);
        }
        public async Task ChangeOpenForRegistrationStatus(int tournamentId, bool newVal)
        {
            await _tournamentRepository.SetFlagValue(tournamentId, nameof(Tournament.IsOpenForRegistration), newVal);
        }
    }
}