using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Core.Teams.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Games.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly IDbConnection _connection;
        public GameRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Game> GetById(int id)
        {
            var teams = new Dictionary<int, Team>();
            var game = (await _connection.QueryAsync<Game, Team, Game>(
                GameQueries.GetById,
                (g, t) =>
                {
                    if (!teams.TryGetValue(t.Id, out var _))
                    {
                        teams.Add(t.Id, t);
                    }
                    return g;
                },
                new { Id = id },
                splitOn: "TeamId"
            )).FirstOrDefault();
            game.Teams = teams.Values.ToList();
            return game;
        }
        public async Task<IEnumerable<Game>> GetByTeamId(int id)
        {
            return (await _connection.QueryAsync<Game, Team, Team, Game>(
                GameQueries.GetByTeamId,
                (g, t1, t2) =>
                {
                    g.Teams = new List<Team> { t1, t2 };
                    return g;
                },
                new { Id = id }
            )).ToList();
        }
        public async Task Create(Game game)
        {
            using var scope = new TransactionScope();

            await _connection.ExecuteAsync(
                GameQueries.Create,
                new { GameRound = game.GameRound }
            );

            await _connection.ExecuteAsync(
                GameQueries.CreateScoresForTeam,
                game.Teams.Select(t => new { TeamId = t.Id })
            );

            scope.Complete();
        }
        public async Task<Game> Update(Game game)
        {
            await _connection.ExecuteAsync(
                GameQueries.UpdateGame,
                new
                {
                    GameRound = game.GameRound,
                    ScoreDifference = game.ScoreDifference,
                    WinnerId = game.Winner.Id,
                    Id = game.Id,
                }
            );

            return await GetById(game.Id);
        }
        public void Delete(Game game)
        {
            throw new System.NotImplementedException();
        }
    }
}