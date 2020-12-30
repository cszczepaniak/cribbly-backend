using System;
using System.Threading.Tasks;
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
    public class PlayerControllerTests
    {
        private readonly Mock<HttpRequest> mockHttpRequest;
        private readonly Mock<HttpContext> mockHttpContext;
        private readonly PlayerController playerController;
        private readonly Mock<IPlayerService> mockPlayerService;

        public PlayerControllerTests()
        {
            mockPlayerService = new Mock<IPlayerService>();
            mockHttpRequest = new Mock<HttpRequest>();
            mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            playerController = new PlayerController(mockPlayerService.Object);
            playerController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenPlayerIsNull()
        {
            mockPlayerService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync((Player)null);
            var result = await playerController.GetById(1);
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task GetById_ShouldReturnPlayerAndOkStatus()
        {
            var expPlayer = new Player()
            {
                Id = 1,
                Email = "test@test.com",
                Name = "test player",
            };
            mockPlayerService.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(expPlayer);
            var result = await playerController.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actPlayer = Assert.IsType<Player>(okResult.Value);
            Assert.Equal(expPlayer.Id, actPlayer.Id);
            Assert.Equal(expPlayer.Email, actPlayer.Email);
            Assert.Equal(expPlayer.Name, actPlayer.Name);
        }
        [Fact]
        public async Task GetByEmail_ShouldReturnBadRequest_WhenHeaderIsOmitted()
        {
            var headers = new HeaderDictionary();
            headers.Append("NotTheRightHeader", "");
            mockHttpRequest.Setup(x => x.Headers).Returns(headers);
            var result = await playerController.GetByEmail();
            var typedResult = Assert.IsType<BadRequestObjectResult>(result);
            var msg = Assert.IsType<string>(typedResult.Value);
            Assert.Equal("`Email` header must be provided", msg);
        }
        [Fact]
        public async Task GetByEmail_ShouldReturnNotFound_WhenPlayerIsNull()
        {
            mockPlayerService.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync((Player)null);

            var headers = new HeaderDictionary();
            headers.Append("Email", "something@something.com");
            mockHttpRequest.Setup(x => x.Headers).Returns(headers);

            var result = await playerController.GetByEmail();
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task GetByEmail_ShouldReturnPlayerAndOkStatus()
        {
            var expPlayer = new Player()
            {
                Id = 1,
                Email = "test@test.com",
                Name = "test player",
            };
            mockPlayerService.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync(expPlayer);

            var headers = new HeaderDictionary();
            headers.Append("Email", expPlayer.Email);
            mockHttpRequest.Setup(x => x.Headers).Returns(headers);

            var result = await playerController.GetByEmail();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actPlayer = Assert.IsType<Player>(okResult.Value);
            Assert.Equal(expPlayer.Id, actPlayer.Id);
            Assert.Equal(expPlayer.Email, actPlayer.Email);
            Assert.Equal(expPlayer.Name, actPlayer.Name);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_IfNoError()
        {
            mockPlayerService.Setup(x => x.Create(It.IsAny<Player>()));

            var result = await playerController.Create(new Player());

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturn500_IfError()
        {
            mockPlayerService.Setup(x => x.Create(It.IsAny<Player>())).Throws(new Exception());

            var result = await playerController.Create(new Player());

            var typedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, typedResult.StatusCode);
        }
    }
}