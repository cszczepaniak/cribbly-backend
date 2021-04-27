using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Threading.Tasks;
using CribblyBackend.Controllers;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;
using CribblyBackend.Services;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Dapper;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class StandingsServiceTests
    {
        private readonly StandingsService StandingsService;
        private readonly Mock<IGameRepository> mockGameRepository;

        public StandingsServiceTests()
        {
            mockGameRepository = new Mock<IGameRepository>();
            StandingsService = new StandingsService(mockGameRepository.Object);
        }
        [Fact]
        public async Task Get_ShouldReturnListsForTeamGameProperties()
        {
            Team team1 = new Team() { Id = 1, Name = "team1"};
            Team team2 = new Team() { Id = 1, Name = "team2"};
            Game game = new Game(){ Teams = new List<Team>(){team1, team2}};
            var games = new List<Game>(){game};
            mockGameRepository.Setup(x => x.GetByTeamId(team1.Id)).ReturnsAsync(games);
            var result = await StandingsService.Calculate(team1);
            Assert.IsType<List<Game>>(result.BracketGames);
            Assert.IsType<List<Game>>(result.PlayInGames);
        }
    }
}