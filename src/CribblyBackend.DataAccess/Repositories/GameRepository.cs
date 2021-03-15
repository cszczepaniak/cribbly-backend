using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Repositories
{
    public interface IGameRepository
    {
        Task<Game> GetById(int Id);
        Task Create(Game Game);
    }
    public class GameRepository : RepositoryBase, IGameRepository
    {
        public GameRepository(IConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        public async Task<Game> GetById(int id)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            var players = new Dictionary<int, Player>();
            var teams = new Dictionary<int, Team>();
            var game = (await connection.QueryAsync<Game, Team, Game>(
                @"
                    SELECT * FROM Scores s 
                    LEFT JOIN Games g on s.GameId = g.Id 
                    LEFT JOIN Teams t on s.TeamId = t.Id 
                    WHERE GameId = @id
                ",
                (g, t) =>
                {
                    if (!teams.TryGetValue(t.Id, out Team _))
                    {
                        teams.Add(t.Id, t);
                    }
                    return g;
                },
                new { Id = id },
                splitOn: "Id"
                )).FirstOrDefault();
            game.Teams = teams.Values.ToList();
            return game;
        }
        public async Task Create(Game game)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            await connection.ExecuteAsync(
                @"INSERT INTO Games(GameRound) VALUES (@GameRound)",
                new { GameRound = game.GameRound }
            );
            foreach (Team team in game.Teams)
            {
                await connection.ExecuteAsync(
                    @"INSERT INTO Scores(GameId, TeamId) VALUES ((SELECT MAX(id) FROM Games), @TeamId)",
                    new { TeamId = team.Id });
            }
        }
    }
}