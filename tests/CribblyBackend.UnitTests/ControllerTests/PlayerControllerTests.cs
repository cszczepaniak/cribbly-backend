using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CribblyBackend.Controllers;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Services;
using CribblyBackend.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Serilog;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class PlayerControllerTests
    {
        private readonly Mock<HttpRequest> mockHttpRequest;
        private readonly Mock<HttpContext> mockHttpContext;
        private readonly PlayerController playerController;
        private readonly Mock<IPlayerService> mockPlayerService;
        private readonly Mock<ILogger> mockLoggerService;

        public PlayerControllerTests()
        {
            mockPlayerService = new Mock<IPlayerService>();
            mockLoggerService = new Mock<ILogger>();
            mockHttpRequest = new Mock<HttpRequest>();
            mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            playerController = new PlayerController(mockPlayerService.Object, mockLoggerService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext.Object
                }
            };
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenPlayerIsNull()
        {
            mockPlayerService.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Player)null);
            var result = await playerController.GetById(1);
            Assert.IsType<NotFoundResult>(result);
            mockPlayerService.VerifyAll();
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
            mockPlayerService.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(expPlayer);
            var result = await playerController.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actPlayer = Assert.IsType<Player>(okResult.Value);
            Assert.Equal(expPlayer.Id, actPlayer.Id);
            Assert.Equal(expPlayer.Email, actPlayer.Email);
            Assert.Equal(expPlayer.Name, actPlayer.Name);
            mockPlayerService.VerifyAll();
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
            mockPlayerService.Setup(x => x.GetByEmailAsync("something@something.com")).ReturnsAsync((Player)null);

            var headers = new HeaderDictionary();
            headers.Append("Email", "something@something.com");
            mockHttpRequest.Setup(x => x.Headers).Returns(headers);

            var result = await playerController.GetByEmail();
            Assert.IsType<NotFoundResult>(result);
            mockPlayerService.VerifyAll();
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
            mockPlayerService.Setup(x => x.GetByEmailAsync("test@test.com")).ReturnsAsync(expPlayer);

            var headers = new HeaderDictionary();
            headers.Append("Email", expPlayer.Email);
            mockHttpRequest.Setup(x => x.Headers).Returns(headers);

            var result = await playerController.GetByEmail();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actPlayer = Assert.IsType<Player>(okResult.Value);
            Assert.Equal(expPlayer.Id, actPlayer.Id);
            Assert.Equal(expPlayer.Email, actPlayer.Email);
            Assert.Equal(expPlayer.Name, actPlayer.Name);
            mockPlayerService.VerifyAll();
        }

        [Fact]
        public async Task Login_ShouldReturn400_IfNoEmail()
        {
            AddTestUser(playerController, "auth id");
            var result = await playerController.Login(new LoginRequest());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldGetPlayerAndReturn200_IfPlayerExists()
        {
            var req = new LoginRequest();
            AddTestUser(playerController, "auth id", "abc@abc.com");
            mockPlayerService.Setup(x => x.ExistsAsync("auth id")).ReturnsAsync(true);
            mockPlayerService.Setup(x => x.GetByEmailAsync("abc@abc.com")).ReturnsAsync(new Player() { });

            var result = Assert.IsType<OkObjectResult>(await playerController.Login(req));
            var response = Assert.IsType<LoginResponse>(result.Value);
            Assert.True(response.IsReturning);
            mockPlayerService.VerifyAll();
        }

        [Fact]
        public async Task Login_ShouldReturn400_IfPlayerDoesNotExistAndNoName()
        {
            var req = new LoginRequest();
            AddTestUser(playerController, "auth id", "abc@abc.com");
            mockPlayerService.Setup(x => x.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            var result = Assert.IsType<BadRequestObjectResult>(await playerController.Login(req));
            mockPlayerService.VerifyAll();
        }

        [Fact]
        public async Task Login_ShouldCreatePlayerAndReturn200_IfPlayerDoesNotExist()
        {
            var req = new LoginRequest()
            {
                Name = "name"
            };
            AddTestUser(playerController, "auth id", "abc@abc.com");
            mockPlayerService.Setup(x => x.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            mockPlayerService.Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Player() { });

            var result = Assert.IsType<OkObjectResult>(await playerController.Login(req));
            var response = Assert.IsType<LoginResponse>(result.Value);
            Assert.False(response.IsReturning);
            mockPlayerService.Verify(x => x.CreateAsync("auth id", "abc@abc.com", req.Name), Times.Once());
        }


        private void AddTestUser(ControllerBase controller, string authId, string email = "")
        {
            var user = NewIdentityUser(authId, email);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        private void AddHeader(ControllerBase controller, string header, string value)
        {
            var ctx = controller.ControllerContext.HttpContext;
            if (controller.ControllerContext.HttpContext == null)
            {
                controller.ControllerContext.HttpContext = new DefaultHttpContext();
            }
            controller.ControllerContext.HttpContext.Request.Headers.Add(header, value);
        }

        private ClaimsPrincipal NewIdentityUser(string authProviderId, string email = "")
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, authProviderId),
            };
            if (!string.IsNullOrEmpty(email))
            {
                claims.Add(new(ClaimTypes.Email, email));
            }
            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }
    }
}