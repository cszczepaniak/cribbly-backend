using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Models;
using CribblyBackend.DataAccess.Tournaments.Repositories;

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
            var tournaments =
                await _tournamentRepository.GetTournamentsWithActiveFlag(nameof(Tournament.IsOpenForRegistration));

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
            var tournamentsWithFlagOn = await _tournamentRepository.GetTournamentsWithActiveFlag(flagName);
            var (canSetValue, errMessage) = CanSetFlag(newVal, flagName, tournamentsWithFlagOn.ToList());
            if (!canSetValue)
            {
                throw new Exception($"{errMessage} [attempted to change {flagName} status of {tournamentId}]");
            }
            await _tournamentRepository.SetFlagValue(tournamentId, flagName, newVal);
        }

        private static (bool, string) CanSetFlag(bool newVal, string flagName, List<Tournament> resultsWithFlagSet)
        {
            if (newVal)
            {
                if (resultsWithFlagSet.Count != 0)
                {
                    // can't set flag is a tournament already has the flag active
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
    }
}