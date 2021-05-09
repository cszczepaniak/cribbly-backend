using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Extensions;
using CribblyBackend.DataAccess.Tournaments.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Tournaments.Repositories
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
            await _connection.ExecuteWithObjectAsync(
                TournamentQueries.Create(date)
            );
            return await _connection.QuerySingleAsync<Tournament>(
                TournamentQueries.GetLast().Sql
            );
        }

        public async Task<IEnumerable<Tournament>> GetTournamentsWithActiveFlag(string flagName)
        {
            return await _connection.QueryWithObjectAsync<Tournament>(
                TournamentQueries.GetAllWithActiveFlag(flagName)
            );
        }

        public async Task SetFlagValue(int tournamentId, string flagName, bool newVal)
        {
            await _connection.ExecuteWithObjectAsync(
                TournamentQueries.SetFlag(tournamentId, flagName, newVal)
            );
        }
    }
}