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
        Task<Tournament> Create(Tournament tournament);
    }
    public class TournamentService : ITournamentService
    {
        private readonly IDbConnection connection;
        public TournamentService(IDbConnection connection)
        {
            this.connection = connection;
        }
        public async Task<Tournament> Create(Tournament tournament)
        {
            await connection.ExecuteAsync(
                @"INSERT INTO Tournaments (Date) VALUES (@Date)",
                new { Date = tournament.Date }
            );
            return (await connection.QueryAsync<Tournament>(
                @"SELECT * FROM Tournaments WHERE Id = LAST_INSERT_ID()"
            )).First();
        }

        public async Task<Tournament> GetNextTournament()
        {
            return (await connection.QueryAsync<Tournament>(
                @"
                SELECT * FROM Tournaments
                ORDER BY Date ASC
                LIMIT 1
                "
            )).FirstOrDefault();
        }
    }
}