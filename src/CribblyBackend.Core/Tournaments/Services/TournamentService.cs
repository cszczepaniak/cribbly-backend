using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Core.Common.Exceptions;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Repositories;
using CribblyBackend.Core.Tournaments.Models;
using CribblyBackend.Core.Tournaments.Repositories;

namespace CribblyBackend.Core.Tournaments.Services
{
    public interface ITournamentService
    {
        Task<Tournament> GetNextTournament();
        Task<Tournament> Create(DateTime date);
        Task ChangeActiveStatus(int tournamentId, bool newVal);
        Task ChangeOpenForRegistrationStatus(int tournamentId, bool newVal);
        Task RegisterPlayerAsync(int tournamentId, int playerId);
    }
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITournamentPlayerRepository _tournamentPlayerRepository;
        private readonly IPlayerRepository _playerRepository;

        public TournamentService(
            ITournamentRepository tournamentRepository,
            IPlayerRepository playerRepository,
            ITournamentPlayerRepository tournamentPlayerRepository
        )
        {
            _tournamentRepository = tournamentRepository;
            _playerRepository = playerRepository;
            _tournamentPlayerRepository = tournamentPlayerRepository;
        }

        public async Task<Tournament> Create(DateTime date)
        {
            return await _tournamentRepository.CreateAsync(date);
        }

        public async Task<Tournament> GetNextTournament()
        {
            var tournaments =
                await _tournamentRepository.GetTournamentsWithActiveFlagAsync(nameof(Tournament.IsOpenForRegistration));

            var numTournaments = tournaments.Count();

            if (numTournaments == 0)
            {
                // No tournament has been scheduled
                return null;
            }

            if (numTournaments > 1)
            {
                throw new Exception("There can't be two tournaments simultaneously open for registration");
            }

            return tournaments.Single();
        }

        public async Task ChangeActiveStatus(int tournamentId, bool newVal)
        {
            await SetFlagValue(tournamentId, nameof(Tournament.IsActive), newVal);
        }
        public async Task ChangeOpenForRegistrationStatus(int tournamentId, bool newVal)
        {
            await SetFlagValue(tournamentId, nameof(Tournament.IsOpenForRegistration), newVal);
        }

        private async Task SetFlagValue(int tournamentId, string flagName, bool newVal)
        {
            var tournamentsWithFlagOn = await _tournamentRepository.GetTournamentsWithActiveFlagAsync(flagName);
            var (canSetValue, errMessage) = CanSetFlag(newVal, flagName, tournamentsWithFlagOn.ToList());
            if (!canSetValue)
            {
                throw new Exception($"{errMessage} [attempted to change {flagName} status of {tournamentId}]");
            }
            await _tournamentRepository.SetFlagValueAsync(tournamentId, flagName, newVal);
        }

        private static (bool, string) CanSetFlag(bool newVal, string flagName, List<Tournament> resultsWithFlagSet)
        {
            if (newVal)
            {
                if (resultsWithFlagSet.Count != 0)
                {
                    // can't set flag if a tournament already has the flag active
                    var idList = string.Join(", ", resultsWithFlagSet.Select(t => t.Id.ToString()));
                    return (false, $"Cannot set {flagName}; it is already set on tournament(s) {idList}");
                }
                // return early; nothing more to check if we're setting to true
                return (true, "");
            }
            // Now we're unsetting a flag
            if (resultsWithFlagSet.Count != 1)
            {
                // There should be exactly one tournament with the flag set if we're unsetting it
                return (false, $"Cannot unset {flagName}; there is not exactly one tournament with this flag set");
            }
            // At this point we know there's only one result so can safely call First()
            var result = resultsWithFlagSet.First();
            var isActiveFlagInvalid =
                flagName == nameof(Tournament.IsActive) && !result.IsActive;
            var isOpenForRegistrationFlagInvalid =
                flagName == nameof(Tournament.IsOpenForRegistration) && !result.IsOpenForRegistration;

            if (isActiveFlagInvalid || isOpenForRegistrationFlagInvalid)
            {
                // The tournament we're unsetting the flag on should have it set
                return (false, $"Cannot unset {flagName}; it isn't set on the specified tournament");
            }
            return (true, "");
        }

        public async Task RegisterPlayerAsync(int tournamentId, int playerId)
        {
            var playerTask = _playerRepository.GetByIdAsync(playerId);
            var tournamentTask = _tournamentRepository.GetByIdAsync(tournamentId);

            await Task.WhenAll(tournamentTask, playerTask);
            if (await playerTask == null)
            {
                throw new EntityNotFoundException<Player>(playerId);
            }
            if (await tournamentTask == null)
            {
                throw new EntityNotFoundException<Tournament>(tournamentId);
            }

            await _tournamentPlayerRepository.CreateAsync(tournamentId, playerId);
        }
    }
}