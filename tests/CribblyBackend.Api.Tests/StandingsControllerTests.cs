using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CribblyBackend.Api.Tests.Common;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Test.Support;
using CribblyBackend.Test.Support.Games.Repositories;
using CribblyBackend.Test.Support.Teams.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CribblyBackend.Api.Tests
{
    public class StandingsControllerTests : ApiTestBase
    {
        private readonly FakeGameRepository _fakeGameRepository;
        private readonly FakeTeamRepository _fakeTeamRepository;
        public StandingsControllerTests()
        {
            _fakeGameRepository = _factory.Services.GetRequiredService<FakeGameRepository>();
            _fakeTeamRepository = _factory.Services.GetRequiredService<FakeTeamRepository>();
        }
        [Fact]
        public async Task Calculate_ShouldReturnOkAndPopulatedTeam()
        {
            var teams = TestData.CreateTeams(3).ToList();
            foreach (var t in teams)
            {
                t.Id = await _fakeTeamRepository.CreateAsync(t);
            }
            var games = new List<Game>
            {
                // team 1's play in games
                new() { Teams = new() { teams[0], teams[1] }, GameRound = Round.Round1 },
                new() { Teams = new() { teams[0], teams[2] }, GameRound = Round.Round3 },
                // another random play in game
                new() { Teams = new() { teams[1], teams[2] }, GameRound = Round.Round2 },
                // team 1's tournament games
                new() { Teams = new() { teams[0], teams[1] }, GameRound = Round.TourneyRound1 },
                // another random tournament game
                new() { Teams = new() { teams[1], teams[2] }, GameRound = Round.SemiFinal },
            };
            foreach (var g in games)
            {
                await _fakeGameRepository.CreateAsync(g);
            }

            var result = await _factory.CreateClient().PostAsJsonAsync("/api/standings", teams[0]);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var actualTeam = await result.Content.ReadFromJsonAsync<Team>();
            Assert.Equal(2, actualTeam.PlayInGames.Count);
            Assert.Single(actualTeam.BracketGames);
        }
    }
}