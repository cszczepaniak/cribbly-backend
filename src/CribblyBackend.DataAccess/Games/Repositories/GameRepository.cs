using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CribblyBackend.Core.Common.Exceptions;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.DataAccess.Extensions;
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

        public async Task<Game> GetByIdAsync(int id)
        {
            var teams = new Dictionary<int, Team>();
            var scores = new Dictionary<int, Score>(2);
            var game = (await _connection.QueryAsync<Score, Game, Team, Game>(
                GameQueries.GetById,
                (s, g, t) =>
                {
                    if (!teams.TryGetValue(t.Id, out var _))
                    {
                        teams.Add(t.Id, t);
                    }
                    scores[s.GameScore] = s;
                    return g;
                },
                new { Id = id }
            )).SingleOrDefault();
            if (game == null)
            {
                return null;
            }
            if (teams.Count != 2)
            {
                throw new UnexpectedGameDataException(game.Id, $"Expected two teams, got {teams.Count}");
            }
            game.Teams = teams.Values.ToList();
            if (scores.Count < 2)
            {
                if (scores.ContainsKey(0))
                {
                    // both scores are zero, nothing else to do
                    return game;
                }
                // why don't we have two scores?
                throw new UnexpectedGameDataException(game.Id, $"Expected 2 unique scores, got {scores.Count}");
            }
            if (scores.Count > 2)
            {
                throw new UnexpectedGameDataException(game.Id, $"Expected 2 unique scores, got {scores.Count}");
            }
            // great, we have exactly two!
            if (!scores.ContainsKey(121))
            {
                // but why wasn't one of them 121?
                var scoreStr = string.Join(" and ", scores.Values.Select(v => v.ToString()));
                throw new UnexpectedGameDataException(game.Id, $"Expected exactly one score to be 121, got {scoreStr}.");
            }
            game.ScoreDifference = Math.Abs(121 - scores.Keys.Where(s => s != 121).Single());
            game.Winner = new() { Id = scores[121].TeamId };
            return game;
        }

        public async Task<IEnumerable<Game>> GetByTeamIdAsync(int id)
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

        public async Task<Game> CreateAsync(Game game)
        {
            using var scope = new TransactionScope();

            await _connection.ExecuteAsync(
                GameQueries.Create,
                new { GameRound = game.GameRound }
            );

            var createdId = await _connection.QueryLastInsertedId();

            await _connection.ExecuteAsync(
                GameQueries.InitializeScoresForTeam,
                game.Teams.Select(t => new { TeamId = t.Id })
            );

            scope.Complete();
            game.Id = createdId;
            return game;
        }
        public async Task<Game> UpdateAsync(Game game)
        {
            if (await GetByIdAsync(game.Id) == null)
            {
                return null;
            }

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

            if (game.Winner != null)
            {
                await _connection.ExecuteAsync(
                    GameQueries.UpdateScoreForTeam,
                    new { Score = 121, TeamId = game.Winner.Id, GameId = game.Id }
                );

                await _connection.ExecuteAsync(
                    GameQueries.UpdateScoreForTeam,
                    new { Score = 121 - game.ScoreDifference, TeamId = game.Teams.FirstOrDefault(t => t.Id != game.Winner.Id).Id, GameId = game.Id }
                );
            }

            return await GetByIdAsync(game.Id);
        }
        public Task DeleteAsync(Game game)
        {
            throw new NotImplementedException();
        }
    }
}