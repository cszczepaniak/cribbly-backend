using System;
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
                @"INSERT INTO Tournaments (Date) VALUES (@Date)",
                new { Date = date }
            );
            return (await connection.QueryAsync<Tournament>(
                @"SELECT * FROM Tournaments WHERE Id = LAST_INSERT_ID()"
            )).First();
        }

        public async Task<Tournament> GetNextTournament()
        {
            var tournaments = await connection.QueryAsync<Tournament>(
                @"
                SELECT * FROM Tournaments
                ORDER BY Date ASC
                "
            );
            return tournaments.Where(t => t.Date.Date >= DateTime.Today).FirstOrDefault();
        }
    }
}