using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CribblyBackend.Controllers;
using CribblyBackend.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class TeamControllerTests
    {
        private readonly Mock<HttpRequest> mockHttpRequest;
        private readonly Mock<HttpContext> mockHttpContext;
        private readonly TeamController TeamController;
        private readonly Mock<ITeamService> mockTeamService;

        public TeamControllerTests()
        {
            mockTeamService = new Mock<ITeamService>();
            mockHttpRequest = new Mock<HttpRequest>();
            mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            TeamController = new TeamController(mockTeamService.Object);
            TeamController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
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
            for(int i = 1; i <= 2; i++)
            {
                Player expPlayer = new Player()
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
        public async Task Create_ShouldReturnOk_IfNoError()
        {
            mockTeamService.Setup(x => x.Create(It.IsAny<Team>()));
            var result = await TeamController.Create(new Team());
            Assert.IsType<OkResult>(result);
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