using System.Data;
using System.Threading.Tasks;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Divisions.Repositories;
using Dapper;

namespace CribblyBackend.DataAccess.Divisions
{
    public class DivisionRepository : IDivisionRepository
    {
        private readonly IDbConnection _connection;
        public DivisionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void GetById(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<Division> Create(Division division)
        {
            var result = await _connection.ExecuteAsync(
                @"INSERT INTO Divisions(Name) VALUES (@Name)",
                new { Name = division.Name }
            );

            return division;
        }
        public void Update(Division division)
        {
            throw new System.NotImplementedException();
        }
        public void Delete(Division division)
        {
            throw new System.NotImplementedException();
        }
    }
}