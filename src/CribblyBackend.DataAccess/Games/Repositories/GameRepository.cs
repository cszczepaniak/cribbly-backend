using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Extensions;
using CribblyBackend.DataAccess.Games.Models;
using CribblyBackend.DataAccess.Players.Models;
using CribblyBackend.DataAccess.Teams.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Games.Repositories
{
    public interface IGameRepository
    {
        Task<Game> GetById(int Id);
        void Update(Game Game);
        Task Create(Game Game);
        void Delete(Game Game);
    }
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