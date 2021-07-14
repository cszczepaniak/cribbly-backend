using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CribblyBackend.Controllers;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Services;
using CribblyBackend.Network;
using CribblyBackend.Test.Support;
using CribblyBackend.Test.Support.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class PlayerControllerTests
    {
        private readonly PlayerController _playerController;
        private readonly IPlayerService _playerService;

        public PlayerControllerTests()
        {
            var provider = ServiceProviderFactory.GetProvider();
            _playerService = provider.GetRequiredService<IPlayerService>();
            var logger = provider.GetRequiredService<ILogger>();
            _playerController = new PlayerController(_playerService, logger);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenPlayerIsNull()
        {
            var result = await _playerController.GetById(1);
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task GetById_ShouldReturnPlayerAndOkStatus()
        {
            var expPlayer = new Player()
            {
                Id = 1,
                AuthProviderId = "authId",
                Email = "test@test.com",
                Name = "test player",
            };
            await _playerService.CreateAsync(expPlayer.AuthProviderId, expPlayer.Email, expPlayer.Name);

            var result = await _playerController.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actPlayer = Assert.IsType<Player>(okResult.Value);
            Assert.Equal(expPlayer.Id, actPlayer.Id);
            Assert.Equal(expPlayer.Email, actPlayer.Email);
            Assert.Equal(expPlayer.Name, actPlayer.Name);
        }
        [Fact]
        public async Task GetByEmail_ShouldReturnBadRequest_WhenNoEmailIsInJWT()
        {
            _playerController.AddTestUser("authId");
            var result = await _playerController.GetByEmail();
            var typedResult = Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task GetByEmail_ShouldReturnNotFound_WhenPlayerIsNull()
        {
            _playerController.AddHeader("Email", "abc@abc.com");
            var result = await _playerController.GetByEmail();
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task GetByEmail_ShouldReturnPlayerAndOkStatus()
        {
            var expPlayer = new Player()
            {
                Id = 1,
                AuthProviderId = "authId",
                Email = "test@test.com",
                Name = "test player",
            };
            await _playerService.CreateAsync(expPlayer.AuthProviderId, expPlayer.Email, expPlayer.Name);

            _playerController.AddHeader("Email", "test@test.com");

            var result = await _playerController.GetByEmail();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actPlayer = Assert.IsType<Player>(okResult.Value);
            Assert.Equal(expPlayer.Id, actPlayer.Id);
            Assert.Equal(expPlayer.Email, actPlayer.Email);
            Assert.Equal(expPlayer.Name, actPlayer.Name);
        }

        [Fact]
        public async Task Login_ShouldReturn400_IfNoEmail()
        {
            _playerController.AddTestUser("authId");
            var result = await _playerController.Login(new LoginRequest());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldGetPlayerAndReturn200_IfPlayerExists()
        {
            _playerController.AddTestUser("auth id", "abc@abc.com");
            await _playerService.CreateAsync("auth id", "abc@abc.com", "dom toretto");

            var result = Assert.IsType<OkObjectResult>(await _playerController.Login(new() { Name = "dom toretto" }));
            var response = Assert.IsType<LoginResponse>(result.Value);
            Assert.True(response.IsReturning);
        }

        [Fact]
        public async Task Login_ShouldReturn400_IfPlayerDoesNotExistAndNoName()
        {
            _playerController.AddTestUser("auth id", "abc@abc.com");

            var result = Assert.IsType<BadRequestObjectResult>(await _playerController.Login(new()));
        }

        [Fact]
        public async Task Login_ShouldCreatePlayerAndReturn200_IfPlayerDoesNotExist()
        {
            var req = new LoginRequest()
            {
                Name = "name"
            };
            _playerController.AddTestUser("auth id", "abc@abc.com");

            var result = Assert.IsType<OkObjectResult>(await _playerController.Login(req));
            var response = Assert.IsType<LoginResponse>(result.Value);
            Assert.Equal("name", response.Player.Name);
            Assert.False(response.IsReturning);
        }
    }
}