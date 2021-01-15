using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Models;
using Dapper;

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
        private readonly IDbConnection connection;
        public TournamentService(IDbConnection connection)
        {
            this.connection = connection;
        }
        public async Task<Tournament> Create(DateTime date)
        {
            await connection.ExecuteAsync(
                @"
                INSERT INTO Tournaments (Date, IsOpenForRegistration, IsActive) 
                VALUES (@Date, FALSE, FALSE)
                ",
                new { Date = date }
            );
            return (await connection.QueryAsync<Tournament>(
                @"SELECT * FROM Tournaments WHERE Id = LAST_INSERT_ID()"
            )).First();
        }

        public async Task<Tournament> GetNextTournament()
        {
            var tournaments = (await connection.QueryAsync<Tournament>(
                @"
                SELECT * FROM Tournaments
                WHERE IsOpenForRegistration = TRUE
                "
            )).ToList();
            if (tournaments.Count == 0)
            {
                // No tournament has been scheduled
                return null;
            }
            if (tournaments.Count > 1)
            {
                throw new Exception("There can't be two tournaments simultaneously open for registration");
            }
            return tournaments.First();
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
            // Note: this breaks the rule of using only parameterized query strings; however, the external world
            // CANNOT control flagName here since flagName is passed by us and never from an external source
            var activeTournaments = (await connection.QueryAsync<Tournament>(
                $@"SELECT * FROM Tournaments WHERE {flagName} = 1", // `true` doesn't exist in mysql; use 1
                new { Name = flagName }
            )).ToList();
            var (canSetValue, errMessage) = CanSetFlag(newVal, flagName, activeTournaments);
            if (!canSetValue)
            {
                throw new Exception($"{errMessage} [attempted to change {flagName} status of {tournamentId}]");
            }
            // Note: this breaks the rule of using only parameterized query strings; however, the external world
            // CANNOT control flagName here since flagName is passed by us and never from an external source
            await connection.ExecuteAsync(
                $@"
                UPDATE Tournaments 
                SET {flagName} = @Value
                WHERE Id = @Id
                ",
                new { Name = flagName, Value = newVal, Id = tournamentId }
                );
        }

        private (bool, string) CanSetFlag(bool newVal, string flagName, List<Tournament> resultsWithFlagSet)
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