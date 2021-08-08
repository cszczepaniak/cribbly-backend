using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CribblyBackend.Api.Tests.Common;
using System.Net.Http.Json;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Test.Support.Games.Repositories;
using CribblyBackend.Test.Support.Teams.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CribblyBackend.Test.Support;
using System.Linq;

namespace CribblyBackend.Api.Tests
{
    public class TeamControllerTests : ApiTestBase
    {
        private readonly FakeGameRepository _fakeGameRepository;
        private readonly FakeTeamRepository _fakeTeamRepository;


        public TeamControllerTests()
        {
            _fakeGameRepository = _factory.Services.GetRequiredService<FakeGameRepository>();
            _fakeTeamRepository = _factory.Services.GetRequiredService<FakeTeamRepository>();
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFoundWhenTeamIsNull()
        {
            var result = await _factory.CreateClient().GetAsync("/api/team/123");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
        [Fact]
        public async Task GetById_ShouldReturnTeamAndOkStatus()
        {
            var expTeam = CreateTestTeams(1).Single();
            var id = await _fakeTeamRepository.CreateAsync(expTeam);

            var result = await _factory.CreateClient().GetAsync($"/api/team/{id}");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var actTeam = await result.Content.ReadFromJsonAsync<Team>();
            Assert.Equal(expTeam.Id, actTeam.Id);
            Assert.Equal(expTeam.Name, actTeam.Name);
            foreach (var p in expTeam.Players)
            {
                Assert.Contains(p.Email, actTeam.Players.Select(p => p.Email));
                Assert.Contains(p.Name, actTeam.Players.Select(p => p.Name));
            }
        }
        [Fact]
        public async Task GetAll_ShouldReturnTeamsAndOkStatus()
        {
            var expTeams = CreateTestTeams(30);
            foreach (var t in expTeams)
            {
                t.Id = await _fakeTeamRepository.CreateAsync(t);
            }

            var result = await _factory.CreateClient().GetAsync("/api/team");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var actTeams = await result.Content.ReadFromJsonAsync<List<Team>>();
            Assert.Equal(30, actTeams.Count);
            foreach (var t in actTeams)
            {
                Assert.Contains(t.Name, actTeams.Select(t => t.Name));
                Assert.Equal(2, t.Players.Count);
            }
        }

        [Fact]
        public async Task Create_ShouldReturnOkAndCreatedId_IfNoError()
        {
            var expTeam = CreateTestTeams(1).Single();
            var result = await _factory.CreateClient().PostAsJsonAsync("/api/team", expTeam);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotEqual("0", await result.Content.ReadAsStringAsync());
            var team = (await _fakeTeamRepository.GetAllAsync()).Single();
            Assert.Equal(expTeam.Name, team.Name);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_IfNoError()
        {
            var expTeam = CreateTestTeams(1).Single();
            var id = await _fakeTeamRepository.CreateAsync(expTeam);

            var result = await _factory.CreateClient().DeleteAsync($"api/team/{id}/delete");
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            Assert.Null(await _fakeTeamRepository.GetByIdAsync(id));
        }

        [Fact]
        public async Task Delete_ShouldReturn404_IfTeamNotFound()
        {
            var result = await _factory.CreateClient().DeleteAsync($"api/team/123");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        private IEnumerable<Team> CreateTestTeams(int n)
        {
            return Enumerable.Range(0, n).Select(_ => new Team
            {
                Name = $"{TestData.NewString()}",
                Players = Enumerable.Range(0, 2).Select(i => new Player
                {
                    Id = i + 1,
                    Email = $"{TestData.NewString()}@test.com",
                    Name = $"{TestData.NewString()}"
                }).ToList(),
            });
        }
    }
}