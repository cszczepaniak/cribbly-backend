using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Repositories
{
    public interface IDivisionRepository
    {
        Task<int> Create(Division division);
        Task<Division> GetById(int id);
    }
    public class DivisionRepository : IDivisionRepository
    {
        private readonly IDbConnection _connection;
        public DivisionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(Division division)
        {
            await _connection.ExecuteAsync(
                @"INSERT INTO Divisions(Name) VALUES (@Name)",
                new { division.Name }
            );
            if (division.Teams.Count > 0)
            {
                using var transaction = _connection.BeginTransaction();
                foreach (var team in division.Teams)
                {
                    await _connection.ExecuteAsync(
                        @"UPDATE Teams SET DivisionId = LAST_INSERT_ID() WHERE Id = @TeamId",
                        new { TeamId = team.Id },
                        transaction
                    );
                }
                transaction.Commit();
            }
            return (await _connection.QueryAsync<int>(@"SELECT LAST_INSERT_ID()")).Single();
        }

        public async Task<Division> GetById(int id)
        {
            var teams = new Dictionary<int, Team>();
            var division = (await _connection.QueryAsync<Division, Team, Division>(
                @"SELECT d.*, t.* FROM Divisions d LEFT JOIN Teams t ON d.Id = t.DivisionId WHERE t.DivisionId = @Id",
                (d, t) =>
                {
                    if (!teams.TryGetValue(t.Id, out Team _))
                    {
                        t.Division = d;
                        teams.Add(t.Id, t);
                    }
                    return d;
                },
                new { Id = id },
                splitOn: "Id"
                )).FirstOrDefault();
            division.Teams = teams.Values.ToList();
            return division;
        }
    }
}