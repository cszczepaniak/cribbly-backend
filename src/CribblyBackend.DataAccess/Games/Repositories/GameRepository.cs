using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.DataAccess.Extensions;
using Dapper;

namespace CribblyBackend.DataAccess.Games.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly IDbConnection connection;
        public GameRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<Game> GetById(int id)
        {
            var players = new Dictionary<int, Player>();
            var teams = new Dictionary<int, Team>();
            var game = (await connection.QueryWithObjectAsync<Game, Team, Game>(
                GameQueries.GetById(id),
                (g, t) =>
                {
                    if (!teams.TryGetValue(t.Id, out Team _))
                    {
                        teams.Add(t.Id, t);
                    }
                    return g;
                }
                )).FirstOrDefault();
            game.Teams = teams.Values.ToList();
            return game;
        }
        public async Task<IEnumerable<Game>> GetByTeamId(int id)
        {
            var games = (await connection.QueryAsync<Game, Team, Team, Game>(
                @"
                    SELECT * FROM Scores s 
                    LEFT JOIN Games g on s.GameId = g.Id 
                    LEFT JOIN Teams t on s.TeamId = t.Id 
                    LEFT JOIN Scores s2 on s.GameId = s2.GameId
                    LEFT JOIN Teams t2 on s2.TeamId = t2.Id
                    WHERE s.TeamId = @id
                    AND s.TeamId != s2.TeamId
                ",
                (g, t, t2) =>
                {
                    g.Teams = new List<Team>();
                    g.Teams.Add(t);
                    g.Teams.Add(t2);
                    return g;
                },
                new { Id = id }
                ));

            return games.ToList();
        }
        public async Task Create(Game game)
        {
            await connection.ExecuteWithObjectAsync(
                GameQueries.Create(game.GameRound)
            );
            foreach (Team team in game.Teams)
            {
                await connection.ExecuteWithObjectAsync(
                    GameQueries.CreateScoresForTeam(team.Id)
                );
            }
        }
        public void Update(Game game)
        {
            throw new System.NotImplementedException();
        }
        public void Delete(Game game)
        {
            throw new System.NotImplementedException();
        }
    }
}