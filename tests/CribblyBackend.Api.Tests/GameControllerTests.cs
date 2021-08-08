using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CribblyBackend.Api.Tests.Common;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Test.Support.Games.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CribblyBackend.Api.Tests
{
    public class GameControllerTests : ApiTestBase
    {
        private readonly FakeGameRepository _fakeGameRepository;


        public GameControllerTests()
        {
            _fakeGameRepository = _factory.Services.GetRequiredService<FakeGameRepository>();
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFoundWhenGameIsNull()
        {
            var result = await _factory.CreateClient().GetAsync($"/api/game/1");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
        [Fact]
        public async Task GetById_ShouldReturnGameAndOkStatus()
        {
            var expGame = new Game()
            {
                GameRound = Round.Round1,
                Teams = Enumerable.Range(0, 2).Select(i => new Team { Id = i + 1 }).ToList(),
            };
            expGame = await _fakeGameRepository.CreateAsync(expGame);

            var result = await _factory.CreateClient().GetAsync($"/api/game/{expGame.Id}");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var actGame = await result.Content.ReadFromJsonAsync<Game>();
            Assert.Equal(expGame.Id, actGame.Id);
            Assert.Equal(2, actGame.Teams.Count);
        }

        [Fact]
        public async Task Get_ShouldReturnBadRequest_IfTeamNotSpecified()
        {
            var result = await _factory.CreateClient().GetAsync("/api/game");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Get_ShouldReturnGamesForTeam()
        {
            foreach (var g in CreateTestGamesForTeam(1234, n: 20))
            {
                await _fakeGameRepository.CreateAsync(g);
            }
            foreach (var g in CreateTestGamesForTeam(1235, n: 25))
            {
                await _fakeGameRepository.CreateAsync(g);
            }
            var result = await _factory.CreateClient().GetAsync("/api/game?team=1234");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var games = await result.Content.ReadFromJsonAsync<List<Game>>();
            Assert.Equal(20, games.Count);

            result = await _factory.CreateClient().GetAsync("/api/game?team=1235");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            games = await result.Content.ReadFromJsonAsync<List<Game>>();
            Assert.Equal(25, games.Count);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_IfNoError()
        {

            var result = await _factory.CreateClient().PostAsJsonAsync("/api/game", new Game { GameRound = Round.Sweet16 });
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var game = await result.Content.ReadFromJsonAsync<Game>();
            Assert.Equal(Round.Sweet16, game.GameRound);
        }

        [Fact]
        public async Task Update_ShouldReturnUpdatedObjectAndOk()
        {
            var game = new Game
            {
                ScoreDifference = 69,
                GameRound = Round.SemiFinal,
            };
            game = await _fakeGameRepository.CreateAsync(game);
            Assert.Equal(69, game.ScoreDifference);
            Assert.Equal(Round.SemiFinal, game.GameRound);

            game.ScoreDifference = 1;
            game.GameRound = Round.Round1;

            var result = await _factory.CreateClient().PutAsJsonAsync($"/api/game/{game.Id}", game);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var actual = await result.Content.ReadFromJsonAsync<Game>();
            Assert.Equal(1, actual.ScoreDifference);
            Assert.Equal(Round.Round1, actual.GameRound);
        }

        [Fact]
        public async Task Update_ShouldReturn404_IfGameNotFound()
        {
            var result = await _factory.CreateClient().PutAsJsonAsync("/api/game/123", new Game());
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        private IEnumerable<Game> CreateTestGamesForTeam(int teamId, int n)
        {
            var rand = new Random();
            Func<int> getOtherId = () =>
            {
                var otherTeamId = rand.Next();
                while (otherTeamId == teamId)
                {
                    otherTeamId = rand.Next();
                }
                return otherTeamId;
            };
            return Enumerable.Range(0, n).Select(_ => new Game
            {
                ScoreDifference = rand.Next(50) + 1,
                GameRound = Round.SemiFinal,
                Teams = new()
                {
                    new() { Id = teamId },
                    new() { Id = getOtherId() },
                }
            });
        }
    }
}