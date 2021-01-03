using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Models;
using Dapper;

namespace CribblyBackend.Services
{
    public interface IGameService
    {
        Task<Game> GetById(int Id);
        void Update(Game Game);
        Task Create(Game Game);
        void Delete(Game Game);
    }
    public class GameService : IGameService
    {
        IDbConnection connection;
        public GameService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<Game> GetById(int id)
        {
            var players = new Dictionary<int, Player>();
            var teams = new Dictionary<int, Team>();
            var game = (await connection.QueryAsync<Game, Team, Player, Game>(
                @"
                    SELECT * FROM Assignments a 
                    LEFT JOIN Games g on a.GameId = g.Id 
                    LEFT JOIN Teams t on a.TeamId = t.Id 
                    LEFT JOIN Players p on p.TeamId = t.Id
                    WHERE GameId = @id
                ", 
                (g, t, p) =>
                {
                    if (!players.TryGetValue(p.Id, out Player _))
                    {
                        p.Team = new Team(){Id = t.Id};
                        players.Add(p.Id, p);
                    }
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
            foreach(Team team in game.Teams)
            {
                team.Players = players.Values.Where(p => p.Team.Id == team.Id).ToList();
            }
            return game;
        }
        public async Task Create(Game game)
        {
            await connection.ExecuteAsync(
                @"INSERT INTO Games(GameRound, Type) VALUES (@GameRound, @Type)", 
                new { GameRound = game.GameRound, Type = game.Type }
            );
            foreach (Team team in game.Teams)
            {
                await connection.ExecuteAsync(
                    @"INSERT INTO Assignments(GameId, TeamId) VALUES ((SELECT MAX(id) FROM Games), @TeamId)", 
                    new { TeamId = team.Id });
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