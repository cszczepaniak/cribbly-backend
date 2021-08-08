using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CribblyBackend.Api.Tests.Common;
using CribblyBackend.Core.Players.Models;
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
        public async Task LoginShouldReturnBadRequest_IfNoEmail()
        {
            var _client = _factory.CreateAuthenticatedClient("authId", "");
            var response = await _client.PostAsJsonAsync("/api/player/login", new LoginRequest());
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task LoginShouldReturnBadRequest_IfNoName()
        {
            var _client = _factory.CreateAuthenticatedClient(TestData.Player());
            var response = await _client.PostAsJsonAsync("/api/player/login", new LoginRequest { Name = "" });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            response = await _client.PostAsJsonAsync("/api/player/login", new LoginRequest());
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task LoginShouldCreateUser_IfUserDoesNotExist()
        {
            var p = TestData.Player();
            var response = await _factory.CreateAuthenticatedClient(p).PostAsJsonAsync("/api/player/login", new LoginRequest { Name = "cool dude" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var player = await response.Content.ReadFromJsonAsync<Player>();
            Assert.Equal(1, player.Id);
            Assert.False(player.IsReturning);
        }

        [Fact]
        public async Task LoginShouldGetUser_IfUserDoesExist()
        {
            var p = await _fakePlayerRepository.CreateAsync(TestData.Player());
            var response = await _factory.CreateAuthenticatedClient(p).PostAsJsonAsync("/api/player/login", new LoginRequest { Name = p.Name });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var player = await response.Content.ReadFromJsonAsync<Player>();
            Assert.NotEqual(0, player.Id);
            Assert.True(player.IsReturning);
        }

        [Fact]
        public async Task GetById_ShouldReturnPlayer()
        {
            var p = await _fakePlayerRepository.CreateAsync(TestData.Player());
            var response = await _factory.CreateClient().GetAsync("/api/player/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var player = await response.Content.ReadFromJsonAsync<Player>();
            Assert.Equal(p.Id, player.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_IfPlayerDoesNotExist()
        {
            var response = await _factory.CreateClient().GetAsync("/api/player/1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnPlayer()
        {
            var p = await _fakePlayerRepository.CreateAsync(TestData.Player());
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Email", p.Email);
            var response = await client.GetAsync("/api/player");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var player = await response.Content.ReadFromJsonAsync<Player>();
            Assert.Equal(p.Id, player.Id);
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnNotFound_IfPlayerDoesNotExist()
        {
            var p = await _fakePlayerRepository.CreateAsync(TestData.Player());
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Email", "not a real email");
            var response = await client.GetAsync("/api/player");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnBadRequest_IfHeaderDoesNotExist()
        {
            var response = await _factory.CreateClient().GetAsync("/api/player");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}