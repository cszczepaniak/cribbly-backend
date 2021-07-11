using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Divisions.Repositories;
using CribblyBackend.DataAccess.Exceptions;
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

        public async Task<Division> GetById(int id)
        {
            List<Team> teams = new List<Team>();
            var result = await _connection.QueryAsync<Division, Team, Division>(
                @"
                    SELECT 
                    divisions.Id,
                    divisions.Name,
                    teams.Id,
                    teams.Name

                    FROM divisions
                    INNER JOIN teams 
                    ON teams.Division = divisions.Id
                    WHERE divisions.Id = @id;
                ",
                (d, t) => 
                {
                    teams.Add(t);
                    return d;
                },
                new {Id = id}
            );
            result.First().Teams = teams;
            return result.First();
        }
        public async Task<Division> Create(Division division)
        {
            var result = await _connection.ExecuteAsync(
                @"INSERT INTO Divisions(Name) VALUES (@Name)",
                new { Name = division.Name }
            );

            return division;
        }
        public async Task<Division> AddTeam(int id, Team team)
        {
            if (GetById(id) == null)
            {
                throw new DivisionNotFoundException(id);
            }

            var result = await _connection.ExecuteAsync(
                @"
                    UPDATE Teams
                    SET Teams.Division = @Id
                    WHERE Teams.Id = @TeamId
                ",
                new {Id = id, TeamId = team.Id}
            );

            return await this.GetById(id);
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