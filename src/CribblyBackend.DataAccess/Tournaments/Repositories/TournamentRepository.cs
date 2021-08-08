using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Models;
using CribblyBackend.Core.Tournaments.Repositories;
using Dapper;

namespace CribblyBackend.DataAccess.Tournaments.Repositories
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly IDbConnection _connection;
        public TournamentRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<Tournament> CreateAsync(DateTime date)
        {
            await _connection.ExecuteAsync(
                TournamentQueries.Create,
                new { Date = date }
            );
            return await _connection.QuerySingleAsync<Tournament>(TournamentQueries.GetLast);
        }

        public async Task<Tournament> GetByIdAsync(int id)
        {
            var tournaments = await _connection.QueryAsync<Tournament>(
                TournamentQueries.GetById,
                new { Id = id }
            );
            return tournaments.Single();
        }

        public async Task<IEnumerable<Tournament>> GetTournamentsWithActiveFlagAsync(string flagName)
        {
            return await _connection.QueryAsync<Tournament>(
                TournamentQueries.GetAllWithActiveFlag(flagName)
            );
        }

        public async Task SetFlagValueAsync(int tournamentId, string flagName, bool newVal)
        {
            await _connection.ExecuteAsync(
                TournamentQueries.SetFlag(flagName),
                new { Id = tournamentId, Value = newVal }
            );
        }
    }
}