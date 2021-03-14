using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Controllers;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class GameControllerTests
    {
        private readonly Mock<HttpRequest> mockHttpRequest;
        private readonly Mock<HttpContext> mockHttpContext;
        private readonly GameController GameController;
        private readonly Mock<IGameService> mockGameService;
        private readonly Mock<ILogger> mockLoggerService;


        public GameControllerTests()
        {
            mockGameService = new Mock<IGameService>();
            mockHttpRequest = new Mock<HttpRequest>();
            mockHttpContext = new Mock<HttpContext>();
            mockLoggerService = new Mock<ILogger>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            GameController = new GameController(mockGameService.Object, mockLoggerService.Object);
            GameController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFoundWhenGameIsNull()
        {
            mockGameService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync((Game)null);
            var result = await GameController.GetById(1);
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task GetById_ShouldReturnGameAndOkStatus()
        {
            var expGame = new Game()
            {
                Id = 1,
                GameRound = (Game.Round)1,
                Teams = new List<Team>()
            };
            for (int i = 1; i <= 2; i++)
            {
                List<Player> playerList = new List<Player>();
                for (int j = 1; j <= 2; j++)
                {
                    Player player = new Player()
                    {
                        Name = $"test player {j}",
                        Email = $"test{i + j}@test.com"
                    };
                    playerList.Add(player);
                }
                Team expTeam = new Team()
                {
                    Id = i,
                    Name = $"test team {i}",
                    Players = playerList
                };
                expGame.Teams.Add(expTeam);
            }
            mockGameService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(expGame);
            var result = await GameController.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actGame = Assert.IsType<Game>(okResult.Value);
            Assert.Equal(expGame.Id, actGame.Id);
            Assert.Equal(expGame.Teams, actGame.Teams);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_IfNoError()
        {
            mockGameService.Setup(x => x.Create(It.IsAny<Game>()));
            var result = await GameController.Create(new Game());
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturn500_IfError()
        {
            mockGameService.Setup(x => x.Create(It.IsAny<Game>())).Throws(new Exception());
            var result = await GameController.Create(new Game());
            var typedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, typedResult.StatusCode);
        }
    }
}