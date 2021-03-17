using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Repositories
{
    public interface IDivisionRepository
    {
        Task<int> Create(string name, IEnumerable<int> teamIds);
        Task<Division> GetById(int id);
    }
    public class DivisionRepository : RepositoryBase, IDivisionRepository
    {
        public DivisionRepository(IConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        public async Task<int> Create(string name, IEnumerable<int> teamIds)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            using var transaction = connection.BeginTransaction();
            try
            {

                await connection.ExecuteAsync(
                    @"INSERT INTO Divisions(Name) VALUES (@Name)",
                    new { Name = name },
                    transaction
                );
                if (teamIds?.Count() > 0)
                {
                    foreach (var id in teamIds)
                    {
                        await connection.ExecuteAsync(
                            @"UPDATE Teams SET DivisionId = LAST_INSERT_ID() WHERE Id = @TeamId",
                            new { TeamId = id },
                            transaction
                        );
                    }
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            transaction.Commit();
            return (await connection.QueryAsync<int>(@"SELECT LAST_INSERT_ID()")).Single();
        }

        public async Task<Division> GetById(int id)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            var teams = new Dictionary<int, Team>();
            var division = (await connection.QueryAsync<Division, Team, Division>(
                @"SELECT d.*, t.* FROM Divisions d LEFT JOIN Teams t ON d.Id = t.DivisionId WHERE t.DivisionId = @Id",
                (d, t) =>
                {
                    if (!teams.TryGetValue(t.Id, out Team _))
                    {
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