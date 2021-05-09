using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Controllers;
using CribblyBackend.DataAccess.Games.Repositories;
using CribblyBackend.DataAccess.Players.Models;
using CribblyBackend.DataAccess.Teams.Models;
using CribblyBackend.DataAccess.Teams.Repositories;
using CribblyBackend.Services;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Dapper;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class TeamServiceTests
    {
        private readonly TeamService teamService;
        private readonly GameService gameService;
        private readonly Mock<ITeamRepository> mockTeamRepository;
        private readonly Mock<IGameRepository> mockGameRepository;

        public TeamServiceTests()
        {
            mockTeamRepository = new Mock<ITeamRepository>();
            mockGameRepository = new Mock<IGameRepository>();
            gameService = new GameService(mockGameRepository.Object);
            teamService = new TeamService(mockTeamRepository.Object, mockGameRepository.Object);
        }
        [Fact]
        public async Task CreateTeamWithOnePlayer_ShouldThrowError()
        {
            Team badTeam = new Team()
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
            List<Team> teams = new List<Team>()
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