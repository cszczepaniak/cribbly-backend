using System;
using CribblyBackend.DataAccess.Common;

namespace CribblyBackend.DataAccess.Tournaments.Repositories
{
    public static class TournamentQueries
    {
        private const string allTournamentColumns = "Id, Date, IsOpenForRegistration, IsActive";
        public static string Create =
            @"INSERT INTO Tournaments (Date, IsOpenForRegistration, IsActive) 
            VALUES (@Date, FALSE, FALSE)";
        public static string GetLast = $"SELECT Id, Date, IsOpenForRegistration, IsActive FROM Tournaments WHERE Id = LAST_INSERT_ID()";

        // Note: this breaks the rule of using only parameterized query strings; however, the external world
        // CANNOT control flagName here since flagName is passed by us and never from an external source
        public static string GetAllWithActiveFlag(string flagName) =>
            $@"SELECT Id, Date, IsOpenForRegistration, IsActive FROM Tournaments WHERE {flagName} = 1"; // `true` doesn't exist in mysql; use 1

        // Note: this breaks the rule of using only parameterized query strings; however, the external world
        // CANNOT control flagName here since flagName is passed by us and never from an external source
        public static string SetFlag(string flagName) =>
            $@"UPDATE Tournaments 
            SET {flagName} = @Value
            WHERE Id = @Id";
    }
}
