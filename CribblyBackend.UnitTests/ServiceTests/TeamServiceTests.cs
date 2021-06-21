using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Core.Games.Services;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Teams.Repositories;
using CribblyBackend.Core.Teams.Services;
using Moq;

using Xunit;

namespace CribblyBackend.UnitTests
{
    public class TeamServiceTests
    {
        private readonly TeamService teamService;
        private readonly GameService gameService;
        private readonly StandingsService standingsService;
        private readonly Mock<ITeamRepository> mockTeamRepository;
        private readonly Mock<IGameRepository> mockGameRepository;
        private readonly Mock<IStandingsService> mockStandingsService;

        public TeamServiceTests()
        {
            mockTeamRepository = new Mock<ITeamRepository>();
            mockGameRepository = new Mock<IGameRepository>();
            mockStandingsService = new Mock<IStandingsService>();
            gameService = new GameService(mockGameRepository.Object);
            teamService = new TeamService(mockTeamRepository.Object, mockStandingsService.Object, mockGameRepository.Object);
            standingsService = new StandingsService(mockGameRepository.Object);
        }
        [Fact]
        public async Task CreateTeamWithOnePlayer_ShouldThrowError()
        {
            var badTeam = new Team()
            {
                Name = "bad",
                Players = new List<Player>() { new Player() { Name = "Tobias Funke" } }
            };
            mockTeamRepository.Setup(x => x.Create(badTeam)).ThrowsAsync(new Exception("A Team must not have less than two players"));
            await Assert.ThrowsAsync<Exception>(async () => await teamService.Create(badTeam));
        }

        [Fact]
        public async Task Get_ShouldNotReturnDuplicateTeams()
        {
            var teams = new List<Team>()
            {
                new Team() { Name = "team1" },
                new Team() { Name = "team2" }
            };

            mockTeamRepository.Setup(x => x.Get()).ReturnsAsync(teams);

            var teamsList = await teamService.Get();
            Assert.True(TeamListHasNoDuplicates(teamsList));
        }
        public bool TeamListHasNoDuplicates(List<Team> teams)
        {
            return teams.GroupBy(t => t.Id).Any(grp => grp.Count() > 1);
        }
    }
}