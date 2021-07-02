namespace CribblyBackend.DataAccess.Players.Repositories
{
    public static class PlayerQueries
    {
        public static string CreatePlayerQuery = @"INSERT INTO Players (Email, Name) VALUES (@Email, @Name)";
        public static string PlayerExistsWithEmail = @"SELECT EXISTS(SELECT Id FROM Players WHERE Email = @Email LIMIT 1)";

        public static string GetById =
            $@"SELECT 
            p.Id, p.Email, p.Name, p.Role, p.TeamId, 
            t.Id, t.Name 
            FROM Players p 
            LEFT JOIN Teams t 
            ON p.TeamId = t.Id 
            WHERE p.Id = @Id";

        public static string GetByEmail =
            $@"SELECT 
            p.Id, p.Email, p.Name, p.Role, p.TeamId, 
            t.Id, t.Name 
            FROM Players p 
            LEFT JOIN Teams t 
            ON p.TeamId = t.Id 
            WHERE p.Email = @Email";
    }
}
