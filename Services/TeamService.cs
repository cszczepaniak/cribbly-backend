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
        void Initialize();
        Task<Team> GetById(int id);
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

        public async Task Create(Team Team)
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
                    Id = Team.Id, 
                    Name = Team.Name, 
                    Division = Team.Division, 
                    Players = JsonConvert.SerializeObject(Team.Players), 
                    GameScores = JsonConvert.SerializeObject(Team.GameScores), 
                    PlayInGames = JsonConvert.SerializeObject(Team.PlayInGames), 
                    BracketGames = JsonConvert.SerializeObject(Team.BracketGames), 
                    Wins = Team.Wins, 
                    Losses = Team.Losses, 
                    TotalScore = Team.TotalScore, 
                    Ranking = Team.Ranking, 
                    Seed = Team.Seed, 
                    InTournament = Team.InTournament
                }
            );
        }
        public void Delete(Team Team)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Team> GetById(int id)
        {
            var Teams = await connection.QueryAsync<Team>(
                @"SELECT * FROM Teams WHERE Id = @Id",
                new { Id = id }
            );
            return Teams.FirstOrDefault();
        }
        /*This table uses the JSON data type. 
        More info: https://dev.mysql.com/doc/refman/5.7/en/json.html
        
        Also note that InTournament will be represented as 0 or 1 in the DB
        */
        public void Initialize()
        {
            connection.Execute(
                @"CREATE TABLE IF NOT EXISTS Teams (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Name VARCHAR(100) NOT NULL,
                    Division VARCHAR(50),
                    Players JSON,
                    GameScores JSON,
                    PlayInGames JSON, 
                    BracketGames JSON,
                    Wins INT, 
                    Losses INT, 
                    TotalScore INT, 
                    Ranking INT, 
                    Seed INT, 
                    InTournament BOOLEAN
                );"
            );
        }

        public void Update(Team Team)
        {
            throw new System.NotImplementedException();
        }
    }
}