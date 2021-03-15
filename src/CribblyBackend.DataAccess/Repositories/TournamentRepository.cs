using System;
using System.Collections.Generic;
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
    public class TournamentRepository : RepositoryBase, ITournamentRepository
    {
        public TournamentRepository(IConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }
        public async Task<Tournament> Create(DateTime date)
        {
            using var connection = _connectionFactory.GetOpenConnection();
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

        public async Task<IEnumerable<Tournament>> GetTournamentsWithActiveFlag(string flagName)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            // Note: this breaks the rule of using only parameterized query strings; however, the external world
            // CANNOT control flagName here since flagName is passed by us and never from an external source
            return await connection.QueryAsync<Tournament>(
                $@"SELECT * FROM Tournaments WHERE {flagName} = 1", // `true` doesn't exist in mysql; use 1
                new { Name = flagName }
            );
        }

        public async Task SetFlagValue(int tournamentId, string flagName, bool newVal)
        {
            using var connection = _connectionFactory.GetOpenConnection();
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
    }
}