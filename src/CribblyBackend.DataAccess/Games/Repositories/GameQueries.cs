namespace CribblyBackend.DataAccess.Games.Repositories
{
    public static class GameQueries
    {
        public static string GetById =
            @"SELECT
                s.TeamId, s.GameScore,
                g.Id, g.GameRound, g.ScoreDifference, g.WinnerId,
                t.Id, t.Name
            FROM Scores s 
            INNER JOIN Games g on s.GameId = g.Id 
            INNER JOIN Teams t on s.TeamId = t.Id 
            WHERE GameId = @Id";

        public static string GetByTeamId =
            @"SELECT s.GameId, s.TeamId, s.GameScore 
            FROM Scores s 
            INNER JOIN Games g on s.GameId = g.Id 
            INNER JOIN Teams t on s.TeamId = t.Id 
            INNER JOIN Scores s2 on s.GameId = s2.GameId
            INNER JOIN Teams t2 on s2.TeamId = t2.Id
            WHERE s.TeamId = @Id
            AND s.TeamId != s2.TeamId";

        public static string Create = @"INSERT INTO Games(GameRound) VALUES (@GameRound)";

        public static string InitializeScoresForTeam =
            @"INSERT INTO Scores(GameId, TeamId, GameScore) VALUES ((SELECT MAX(id) FROM Games), @TeamId, 0)";

        public static string UpdateScoreForTeam =
            @"UPDATE Scores s
            SET s.GameScore = @Score
            WHERE s.GameId = @GameId
            AND s.TeamId = @TeamId";

        public static string UpdateGame =
            @"UPDATE Games g
            SET
                g.GameRound = @GameRound,
                g.WinnerId = @WinnerId,
                g.ScoreDifference = @ScoreDifference
            WHERE g.Id = @Id";
    }
}
