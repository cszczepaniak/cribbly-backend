using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;
using CribblyBackend.Services;
using Moq;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class StandingsServiceTests
    {
        private readonly StandingsService StandingsService;
        private readonly Mock<IGameRepository> mockGameRepository;

        private Team team1 = new Team() { Id = 1, Name = "team1"};
        private Team team2 = new Team() { Id = 1, Name = "team2"};

        public StandingsServiceTests()
        {
            mockGameRepository = new Mock<IGameRepository>();
            StandingsService = new StandingsService(mockGameRepository.Object);
        }
        [Fact]
        public async Task Calculate_ShouldReturnListsForTeamGameProperties()
        {
            Game game = new Game(){ Teams = new List<Team>(){team1, team2}};
            var games = new List<Game>(){game};
            mockGameRepository.Setup(x => x.GetByTeamId(team1.Id)).ReturnsAsync(games);
            var result = await StandingsService.Calculate(team1);
            Assert.IsType<List<Game>>(result.BracketGames);
            Assert.IsType<List<Game>>(result.PlayInGames);
        }
        [Fact]
        public async Task Calculate_ShouldReturnTeamWithOneWinIfTheyHaveLoss()
        {
            Game game = new Game(){ Teams = new List<Team>(){team1, team2}, Winner = team2};
            var games = new List<Game>(){game};
            mockGameRepository.Setup(x => x.GetByTeamId(team1.Id)).ReturnsAsync(games);
            var result = await StandingsService.Calculate(team1);
            Assert.Equal(0, result.Wins);
            Assert.Equal(1, result.Losses);
        }
    }
}