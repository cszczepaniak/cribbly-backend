using System.Data;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Repositories;
using Dapper;

namespace CribblyBackend.DataAccess.Tournaments.Repositories
{
    public class TournamentPlayerRepository : ITournamentPlayerRepository
    {

        private readonly IDbConnection _connection;

        public TournamentPlayerRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task CreateAsync(int tournamentId, int playerId)
        {
            await _connection.ExecuteAsync(
                TournamentPlayerQueries.CreateTournamentPlayerAssociation,
                new { TournamentId = tournamentId, PlayerId = playerId }
            );
        }

        public async Task DeleteAsync(int tournamentId, int playerId)
        {
            await _connection.ExecuteAsync(
                TournamentPlayerQueries.DeleteTournamentPlayerAssociation,
                new { TournamentId = tournamentId, PlayerId = playerId }
            );
        }
    }
}