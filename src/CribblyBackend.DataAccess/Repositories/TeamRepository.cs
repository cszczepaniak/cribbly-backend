using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using Dapper;

namespace CribblyBackend.DataAccess.Repositories
{
    public interface ITeamRepository
    {
        Task<Team> GetById(int Id);
        Task<int> Create(Team Team);
        Task AddToDivision(Team team, int divisionId);
    }
    public class TeamRepository : RepositoryBase, ITeamRepository
    {
        public TeamRepository(IConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        public async Task AddToDivision(Team team, int divisionId)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            await connection.ExecuteAsync(
                @"UPDATE Teams SET DivisionId = @DivisionId WHERE Id = @TeamId",
                new { DivisionId = divisionId, TeamId = team.Id }
            );
        }

        public async Task<int> Create(Team team)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            // TODO move this to service
            if (team.Players.Count < 2)
            {
                throw new System.Exception("A Team must not have less than two players");
            }
            await connection.ExecuteAsync(
                @"INSERT INTO Teams(Name) VALUES (@Name)",
                new { Name = team.Name }
            );
            foreach (Player player in team.Players)
            {
                await connection.ExecuteAsync(
                    @"UPDATE Players SET TeamId = LAST_INSERT_ID() WHERE Id = @PlayerId",
                    new { PlayerId = player.Id });
            }
            return (await connection.QueryAsync<int>(@"SELECT LAST_INSERT_ID()")).First();
        }

        public async Task<Team> GetById(int id)
        {
            using var connection = _connectionFactory.GetOpenConnection();
            var players = new Dictionary<int, Player>();
            var team = (await connection.QueryAsync<Team, Player, Team>(
                @"SELECT t.*, p.* FROM Teams t INNER JOIN Players p ON t.Id = p.TeamId WHERE p.TeamId = @Id",
                (t, p) =>
                {
                    if (!players.TryGetValue(p.Id, out Player _))
                    {
                        players.Add(p.Id, p);
                    }
                    return t;
                },
                new { Id = id },
                splitOn: "Id"
                )).FirstOrDefault();
            team.Players = players.Values.ToList();
            return team;
        }
    }
}