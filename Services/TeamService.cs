using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Models;
using Dapper;
using Newtonsoft.Json;

namespace CribblyBackend.Services
{
    public interface ITeamService
    {
        Task<Team> GetById(int Id);
        void Update(Team Team);
        Task Create(Team Team);
        void Delete(Team Team);
    }
    public class TeamService : ITeamService
    {
        IDbConnection connection;
        public TeamService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task Create(Team team)
        {
            await connection.ExecuteAsync(
                @"INSERT INTO Teams(
                    Id, 
                    Name, 
                    Division, 
                    Players, 
                    GameScores, 
                    PlayInGames, 
                    BracketGames, 
                    Wins, 
                    Losses, 
                    TotalScore, 
                    Ranking, 
                    Seed, 
                    InTournament
                )
                VALUES (
                    @Id, 
                    @Name, 
                    @Division, 
                    @Players, 
                    @GameScores, 
                    @PlayInGames, 
                    @BracketGames, 
                    @Wins, 
                    @Losses, 
                    @TotalScore, 
                    @Ranking, 
                    @Seed, 
                    @InTournament
                )",
                new { 
                    Id = team.Id, 
                    Name = team.Name, 
                    Division = team.Division, 
                    Players = JsonConvert.SerializeObject(team.Players), 
                    GameScores = JsonConvert.SerializeObject(team.GameScores), 
                    PlayInGames = JsonConvert.SerializeObject(team.PlayInGames), 
                    BracketGames = JsonConvert.SerializeObject(team.BracketGames), 
                    Wins = team.Wins, 
                    Losses = team.Losses, 
                    TotalScore = team.TotalScore, 
                    Ranking = team.Ranking, 
                    Seed = team.Seed, 
                    InTournament = team.InTournament
                }
            );
        }
        public void Delete(Team team)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Team> GetById(int id)
        {
            var teams = await connection.QueryAsync<Team>(
                @"SELECT * FROM Teams WHERE Id = @Id",
                new { Id = id }
            );
            return teams.FirstOrDefault();
        }
        public void Update(Team team)
        {
            throw new System.NotImplementedException();
        }
    }
}