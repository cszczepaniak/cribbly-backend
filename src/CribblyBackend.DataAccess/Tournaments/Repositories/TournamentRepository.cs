using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Models;
using CribblyBackend.Core.Tournaments.Repositories;
using CribblyBackend.DataAccess.Common;
using CribblyBackend.DataAccess.Extensions;
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
        public async Task<Tournament> Create(DateTime date)
        {
            await _connection.ExecuteAsync(
                TournamentQueries.Create,
                Query.Params("@Date", date)
            );
            return await _connection.QuerySingleAsync<Tournament>(TournamentQueries.GetLast);
        }

        public async Task<IEnumerable<Tournament>> GetTournamentsWithActiveFlag(string flagName)
        {
            return await _connection.QueryAsync<Tournament>(
                TournamentQueries.GetAllWithActiveFlag(flagName)
            );
        }

        public async Task SetFlagValue(int tournamentId, string flagName, bool newVal)
        {
            await _connection.ExecuteAsync(
                TournamentQueries.SetFlag(flagName),
                Query.Params("@Id", tournamentId, "@Value", newVal)
            );
        }
    }
}