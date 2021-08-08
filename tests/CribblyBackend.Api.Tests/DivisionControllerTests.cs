using System.Threading.Tasks;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Teams.Models;
using Microsoft.AspNetCore.Http;
using Xunit;
using CribblyBackend.Test.Support.Divisions.Repositories;
using CribblyBackend.Api.Tests.Common;
using Microsoft.Extensions.DependencyInjection;
using CribblyBackend.Test.Support;
using System.Net.Http.Json;
using System.Net;
using CribblyBackend.Test.Support.Extensions;

namespace CribblyBackend.Api.Tests
{
    public class DivisionControllerTests : ApiTestBase
    {
        private readonly FakeDivisionRepository _fakeDivisionRepository;
        private Team _team1 = new() { Id = 1, Name = "team1" };
        private Team _team2 = new() { Id = 2, Name = "team2" };
        private Division _division = new() { Id = 1, Name = "Test", Teams = new() };


        public DivisionControllerTests()
        {
            _fakeDivisionRepository = _factory.Services.GetRequiredService<FakeDivisionRepository>();
        }

        [Fact]
        public async Task Create_ShouldReturnOk_IfNoError()
        {
            var division = new Division { Name = TestData.String() };

            var result = await _factory.CreateClient().PostAsJsonAsync("/api/division", division);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var created = await result.Content.ReadFromJsonAsync<Division>();
            Assert.Equal(division.Name, created.Name);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_IfNoError()
        {
            var expDivision = await _fakeDivisionRepository.CreateAsync(new() { Name = TestData.String() });
            var result = await _factory.CreateClient().GetAsync($"/api/division/{expDivision.Id}");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var actual = await result.Content.ReadFromJsonAsync<Division>();
            Assert.Equal(expDivision.Name, actual.Name);
        }

        [Fact]
        public async Task GetById_ShouldReturn404_IfDivisionNotFound()
        {
            var result = await _factory.CreateClient().GetAsync($"/api/division/123");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task AddTeam_ShouldReturnOk_IfNoError()
        {
            var division = await _fakeDivisionRepository.CreateAsync(new() { Name = TestData.String() });
            var team = new Team { Id = 1, Name = TestData.String() };

            var result = await _factory.CreateClient().PatchAsJsonAsync($"/api/division/{division.Id}/team", team);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var actualDivision = await result.Content.ReadFromJsonAsync<Division>();
            Assert.Equal(1, actualDivision.Teams.Count);
        }

        [Fact]
        public async Task AddTeam_ShouldReturn404_IfDivisionNotFound()
        {
            var team = new Team { Id = 1, Name = TestData.String() };
            var result = await _factory.CreateClient().PatchAsJsonAsync($"/api/division/1234/team", team);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}