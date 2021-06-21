using System;
using System.Linq;
using CribblyBackend.DataAccess.Common;
using CribblyBackend.DataAccess.Players.Repositories;

namespace CribblyBackend.DataAccess.Teams.Repositories
{
    public static class TeamQueries
    {
        public static string TeamFields(string prefix)
        {
            var fields = new[] { "Id", "Name", "Division" }.Select(f => prefix + f);
            return string.Join(", ", fields);
        }

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
                Sql = @"UPDATE Players SET TeamId = LAST_INSERT_ID() WHERE Id = @Id",
                Params = new { Id = playerId },
            };
        }
        public static Query GetById(int id)
        {
            return new()
            {
                Sql = $@"SELECT {TeamFields("t.")}, {PlayerQueries.PlayerFields("p.")} FROM Teams t LEFT JOIN Players p ON t.Id = p.TeamId WHERE t.Id = @Id",
                Params = new { Id = id },
            };
        }
        public static Query GetAll()
        {
            return new()
            {
                Sql = $@"SELECT {TeamFields("t.")}, {PlayerQueries.PlayerFields("p.")} FROM Teams t LEFT JOIN Players p ON t.Id = p.TeamId",
            };
        }
    }
}
