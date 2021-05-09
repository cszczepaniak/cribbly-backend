using System;
using CribblyBackend.DataAccess.Common;

namespace CribblyBackend.DataAccess.Tournaments.Repositories
{
    public static class TournamentQueries
    {
        public static Query Create(DateTime date)
        {
            return new()
            {
                Sql = @"
                INSERT INTO Tournaments (Date, IsOpenForRegistration, IsActive) 
                VALUES (@Date, FALSE, FALSE)
                ",
                Params = new { Date = date },
            };
        }
        public static Query GetLast()
        {
            // Note: this breaks the rule of using only parameterized query strings; however, the external world
            // CANNOT control flagName here since flagName is passed by us and never from an external source
            return new()
            {
                Sql = @"SELECT * FROM Tournaments WHERE Id = LAST_INSERT_ID()",
            };
        }
        public static Query GetAllWithActiveFlag(string flagName)
        {
            return new()
            {
                Sql = $@"SELECT * FROM Tournaments WHERE {flagName} = 1", // `true` doesn't exist in mysql; use 1
                Params = new { Name = flagName }
            };
        }
        public static Query SetFlag(int tournamentId, string flagName, bool flagVal)
        {
            // Note: this breaks the rule of using only parameterized query strings; however, the external world
            // CANNOT control flagName here since flagName is passed by us and never from an external source
            return new()
            {
                Sql = $@"
                UPDATE Tournaments 
                SET {flagName} = @Value
                WHERE Id = @Id
                ",
                Params = new { Name = flagName, Value = flagVal, Id = tournamentId }
            };
        }
    }
}
