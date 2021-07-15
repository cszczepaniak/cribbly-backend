using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CribblyBackend.Api.Tests.Common;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Network;
using CribblyBackend.Test.Support;
using Xunit;

namespace CribblyBackend.Api.Tests
{
    public class PlayerControllerTests : ApiTestBase
    {
        public PlayerControllerTests(ApiWebApplicationFactory fixture) : base(fixture) { }

        [Fact]
        public async Task LoginShouldReturnUnauthorizedWhenNoUserExists()
        {
            var response = await _factory.CreateClient().PostAsJsonAsync("/api/player/login", new LoginRequest { Name = "long john silver" });
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task LoginShouldCreateUser()
        {
            var authId = TestData.NewString();
            var email = "cool@cool.com";
            var response = await _factory.CreateClientWithTestAuth(authId, email).PostAsJsonAsync("/api/player/login", new LoginRequest { Name = "cool dude" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var player = await response.Content.ReadFromJsonAsync<Player>();
            Assert.Equal(1, player.Id);
        }
    }
}