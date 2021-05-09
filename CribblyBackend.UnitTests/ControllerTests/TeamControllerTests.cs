using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Controllers;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class TeamControllerTests
    {
        private readonly Mock<HttpRequest> mockHttpRequest;
        private readonly Mock<HttpContext> mockHttpContext;
        private readonly TeamController TeamController;
        private readonly Mock<ITeamService> mockTeamService;
        private readonly Mock<ILogger> mockLoggerService;


        public TeamControllerTests()
        {
            mockTeamService = new Mock<ITeamService>();
            mockLoggerService = new Mock<ILogger>();
            mockHttpRequest = new Mock<HttpRequest>();
            mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            TeamController = new TeamController(mockTeamService.Object, mockLoggerService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFoundWhenTeamIsNull()
        {
            mockTeamService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync((Team)null);
            var result = await TeamController.GetById(1);
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task GetById_ShouldReturnTeamAndOkStatus()
        {
            var expTeam = new Team()
            {
                Id = 1,
                Name = "test Team",
                Players = new List<Player>()
            };
            for (int i = 1; i <= 2; i++)
            {
                var expPlayer = new Player()
                {
                    Id = i,
                    Email = $"test{i}@test.com",
                    Name = $"test player {i}"
                };
                expTeam.Players.Add(expPlayer);
            }
            mockTeamService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(expTeam);
            var result = await TeamController.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actTeam = Assert.IsType<Team>(okResult.Value);
            Assert.Equal(expTeam.Id, actTeam.Id);
            Assert.Equal(expTeam.Name, actTeam.Name);
            Assert.Equal(expTeam.Players, actTeam.Players);
        }
        [Fact]
        public async Task GetAll_ShouldReturnTeamsAndOkStatus()
        {
            var expTeams = new List<Team>();
            for (int i = 1; i <= 30; i++)
            {
                var team = new Team()
                {
                    Id = i,
                    Name = $"test Team {i}",
                    Players = new List<Player>()
                };
                for (int j = 1; j <= 2; j++)
                {
                    var expPlayer = new Player()
                    {
                        Id = i,
                        Email = $"test{i}@test.com",
                        Name = $"test player {i}"
                    };
                    team.Players.Add(expPlayer);
                }
            }


            mockTeamService.Setup(x => x.Get()).ReturnsAsync(expTeams);
            var result = await TeamController.Get();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actTeams = Assert.IsType<List<Team>>(okResult.Value);
            Assert.Equal(expTeams, actTeams);
            foreach (Team team in actTeams)
            {
                Assert.Equal(2, team.Players.Count);
            }
        }

        [Fact]
        public async Task Create_ShouldReturnOkAndCreatedId_IfNoError()
        {
            mockTeamService.Setup(x => x.Create(It.IsAny<Team>())).ReturnsAsync(123);
            var result = await TeamController.Create(new Team());
            var typedResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(123, typedResult.Value);
        }

        [Fact]
        public async Task Create_ShouldReturn500_IfError()
        {
            mockTeamService.Setup(x => x.Create(It.IsAny<Team>())).Throws(new Exception());
            var result = await TeamController.Create(new Team());
            var typedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, typedResult.StatusCode);
        }
    }
}