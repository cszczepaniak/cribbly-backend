using System;
using CribblyBackend.DataAccess.Common;

namespace CribblyBackend.DataAccess.Teams.Repositories
{
    public static class TeamQueries
    {
        public static Query CreateWithName(string name)
        {
            return new()
            {
                Sql = @"INSERT INTO Teams(Name) VALUES (@Name)",
                Params = new { Name = name },
            };
        }
        public static Query UpdatePlayerWithLastTeamId(int playerId)
        {
            return new()
            {
                Sql = @"UPDATE Players SET TeamId = LAST_INSERT_ID() WHERE Id = @PlayerId",
                Params = new { Id = playerId },
            };
        }
        public static Query GetById(int id)
        {
            return new()
            {
                Sql = @"SELECT t.*, p.* FROM Teams t INNER JOIN Players p ON t.Id = p.TeamId WHERE p.TeamId = @Id",
                Params = new { Id = id },
            };
        }
        public static Query GetAll()
        {
            return new()
            {
                Sql = @"SELECT * FROM Teams t INNER JOIN Players p ON t.Id = p.TeamId",
            };
        }
    }
}
