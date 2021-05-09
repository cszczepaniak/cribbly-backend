using System;
using CribblyBackend.DataAccess.Common;

namespace CribblyBackend.DataAccess.Teams.Repositories
{
    public static class TeamQueries
    {
        private const string CreateTeamWithNameQuery = @"INSERT INTO Teams(Name) VALUES (@Name)";
        public static Query CreateWithName(string name)
        {
            return new()
            {
                Sql = CreateTeamWithNameQuery,
                Params = new { Name = name },
            };
        }
        private const string UpdatePlayerTeamToLastIdQuery
            = @"UPDATE Players SET TeamId = LAST_INSERT_ID() WHERE Id = @PlayerId";
        public static Query UpdatePlayerWithLastTeamId(int playerId)
        {
            return new()
            {
                Sql = UpdatePlayerTeamToLastIdQuery,
                Params = new { Id = playerId },
            };
        }

        private const string GetByIdQuery
             = @"SELECT t.*, p.* FROM Teams t INNER JOIN Players p ON t.Id = p.TeamId WHERE p.TeamId = @Id";
        public static Query GetById(int id)
        {
            return new()
            {
                Sql = GetByIdQuery,
                Params = new { Id = id },
            };
        }
        private const string GetAllQuery
            = @"SELECT * FROM Teams t INNER JOIN Players p ON t.Id = p.TeamId";
        public static Query GetAll()
        {
            return new()
            {
                Sql = GetAllQuery,
            };
        }
    }
}
