using System;
using CribblyBackend.DataAccess.Common;

namespace CribblyBackend.DataAccess.Players.Repositories
{
    public static class PlayerQueries
    {
        public static Query CreatePlayerQuery(string email, string name)
        {
            return new()
            {
                Sql = @"INSERT INTO Players (Email, Name) VALUES (@Email, @Name)",
                Params = new { Email = email, Name = name },
            };
        }
        public static Query PlayerExistsWithEmail(string email)
        {
            return new()
            {
                Sql = @"SELECT EXISTS(SELECT * FROM Players WHERE Email = @Email LIMIT 1)",
                Params = new { Email = email },
            };
        }

        public static Query GetById(int id)
        {
            return new()
            {
                Sql = @"SELECT * FROM Players p LEFT JOIN Teams t ON p.TeamId = t.Id WHERE p.Id = @Id",
                Params = new { Id = id },
                SplitOn = "TeamId",
            };
        }

        public static Query GetByEmail(string email)
        {
            return new()
            {
                Sql = @"SELECT * FROM Players p LEFT JOIN Teams t ON p.TeamId = t.Id WHERE p.Email = @Email",
                Params = new { Email = email },
                SplitOn = "TeamId",
            };
        }
    }
}
