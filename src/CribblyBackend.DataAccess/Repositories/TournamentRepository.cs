using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Repositories
{
    public interface ITournamentRepository
    {
        Task<Tournament> Create(DateTime date);
        Task SetFlagValue(int tournamentId, string flagName, bool newVal);
        Task<IEnumerable<Tournament>> GetTournamentsWithActiveFlag(string flagName);
    }
    public class TournamentRepository : ITournamentRepository
    {
        private readonly IDbConnection _connection;
        public TournamentRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<Tournament> Create(DateTime date)
        {
            await _connection.ExecuteAsync(
                @"
                INSERT INTO Tournaments (Date, IsOpenForRegistration, IsActive) 
                VALUES (@Date, FALSE, FALSE)
                ",
                new { Date = date }
            );
            return (await _connection.QueryAsync<Tournament>(
                @"SELECT * FROM Tournaments WHERE Id = LAST_INSERT_ID()"
            )).First();
        }

        public async Task<IEnumerable<Tournament>> GetTournamentsWithActiveFlag(string flagName)
        {
            // Note: this breaks the rule of using only parameterized query strings; however, the external world
            // CANNOT control flagName here since flagName is passed by us and never from an external source
            return await _connection.QueryAsync<Tournament>(
                $@"SELECT * FROM Tournaments WHERE {flagName} = 1", // `true` doesn't exist in mysql; use 1
                new { Name = flagName }
            );
        }

        public async Task SetFlagValue(int tournamentId, string flagName, bool newVal)
        {
            // Note: this breaks the rule of using only parameterized query strings; however, the external world
            // CANNOT control flagName here since flagName is passed by us and never from an external source
            await _connection.ExecuteAsync(
                $@"
                UPDATE Tournaments 
                SET {flagName} = @Value
                WHERE Id = @Id
                ",
                new { Name = flagName, Value = newVal, Id = tournamentId }
                );
        }
    }
}