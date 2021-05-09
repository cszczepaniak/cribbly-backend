using CribblyBackend.Core.Games.Models;
using CribblyBackend.DataAccess.Common;

namespace CribblyBackend.DataAccess.Games.Repositories
{
    public static class GameQueries
    {
        public static Query GetById(int id)
        {
            return new()
            {
                Sql = @"
                    SELECT * FROM Scores s 
                    LEFT JOIN Games g on s.GameId = g.Id 
                    LEFT JOIN Teams t on s.TeamId = t.Id 
                    WHERE GameId = @id
                ",
                Params = new { Id = id },
                SplitOn = "Id",
            };
        }
        public static Query Create(Round round)
        {
            return new()
            {
                Sql = @"INSERT INTO Games(GameRound) VALUES (@GameRound)",
                Params = new { GameRound = round },
            };
        }
        public static Query CreateScoresForTeam(int id)
        {
            return new()
            {
                Sql = @"INSERT INTO Scores(GameId, TeamId) VALUES ((SELECT MAX(id) FROM Games), @TeamId)",
                Params = new { TeamId = id },
            };
        }
    }
}
