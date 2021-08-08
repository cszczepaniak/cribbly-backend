using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Teams.Services;
using Moq;
using Xunit;

namespace CribblyBackend.Core.UnitTests.Standings.Services
{
    public class StandingsServiceTests
    {
        private readonly StandingsService StandingsService;
        private readonly Mock<IGameRepository> mockGameRepository;

        private Team team1 = new() { Id = 1, Name = "team1" };
        private Team team2 = new() { Id = 2, Name = "team2" };

        public StandingsServiceTests()
        {
            mockGameRepository = new Mock<IGameRepository>();
            StandingsService = new StandingsService(mockGameRepository.Object);
        }
        [Fact]
        public async Task Calculate_ShouldReturnListsForTeamGameProperties()
        {
            Game game = new() { Teams = new() { team1, team2 } };
            var games = new List<Game>() { game };
            mockGameRepository.Setup(x => x.GetByTeamIdAsync(team1.Id)).ReturnsAsync(games);
            var result = await StandingsService.Calculate(team1);
            Assert.IsType<List<Game>>(result.BracketGames);
            Assert.IsType<List<Game>>(result.PlayInGames);
        }
        [Fact]
        public async Task Calculate_ShouldReturnTeamWithOneLossIfTheyHaveLoss()
        {
            Game game = new() { Teams = new() { team1, team2 }, Winner = team2 };
            mockGameRepository.Setup(x => x.GetByTeamIdAsync(team1.Id)).ReturnsAsync(new List<Game>() { game });
            var result = await StandingsService.Calculate(team1);
            Assert.Equal(0, result.Wins);
            Assert.Equal(1, result.Losses);
        }
    }
}