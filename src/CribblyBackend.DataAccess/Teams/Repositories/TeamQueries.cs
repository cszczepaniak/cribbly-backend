namespace CribblyBackend.DataAccess.Teams.Repositories
{
    public static class TeamQueries
    {
        public static string CreateWithName = @"INSERT INTO Teams(Name) VALUES (@Name)";
        public static string UpdatePlayerTeamToLastTeamId = @"UPDATE Players SET TeamId = LAST_INSERT_ID() WHERE Id = @PlayerId";
        public static string GetById = $@"
            SELECT 
                t.Id, t.Name, 
                p.Id, p.Email, p.Name, p.Role, p.TeamId, 
                d.Id, d.Name
            FROM Teams t 
            LEFT JOIN Players p 
            ON t.Id = p.TeamId
            LEFT JOIN Divisions d
            ON t.Division = d.Id 
            WHERE t.Id = @Id";
        public static string GetAll = @"
            SELECT 
            t.Id, t.Name, 
            p.Id, p.Email, p.Name, p.Role, p.TeamId 
            FROM Teams t 
            LEFT JOIN Players p 
            ON t.Id = p.TeamId";
    }
}
