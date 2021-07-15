using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CribblyBackend.Api.Tests.Common;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Repositories;
using CribblyBackend.Network;
using CribblyBackend.Test.Support;
using CribblyBackend.Test.Support.Players.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CribblyBackend.Api.Tests
{
    public class PlayerControllerTests : ApiTestBase
    {
        private readonly FakePlayerRepository _fakePlayerRepository;
        public PlayerControllerTests()
        {
            _fakePlayerRepository = _factory.Services.GetRequiredService<FakePlayerRepository>();
        }

        [Fact]
        public async Task LoginShouldReturnUnauthorizedWhenNoUserExists()
        {
            var response = await _factory.CreateClient().PostAsJsonAsync("/api/player/login", new LoginRequest { Name = "long john silver" });
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task LoginShouldCreateUser_IfUserDoesNotExist()
        {
            var authId = TestData.NewString();
            var email = "cool@cool.com";
            var response = await _factory.CreateAuthenticatedClient(authId, email).PostAsJsonAsync("/api/player/login", new LoginRequest { Name = "cool dude" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var player = await response.Content.ReadFromJsonAsync<Player>();
            Assert.Equal(1, player.Id);
            Assert.False(player.IsReturning);
        }

        [Fact]
        public async Task LoginShouldGetUser_IfUserDoesExist()
        {
            await _fakePlayerRepository.CreateAsync(TestData.NewPlayer());
            var p = await _fakePlayerRepository.CreateAsync(new Player { AuthProviderId = TestData.NewString(), Email = "cool@cool.com", Name = "cool dude" });
            var response = await _factory.CreateAuthenticatedClient(p).PostAsJsonAsync("/api/player/login", new LoginRequest { Name = p.Name });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var player = await response.Content.ReadFromJsonAsync<Player>();
            Assert.Equal(2, player.Id);
            Assert.True(player.IsReturning);
        }
    }
}