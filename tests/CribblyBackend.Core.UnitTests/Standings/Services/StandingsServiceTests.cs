using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Teams.Services;
using CribblyBackend.Test.Support;
using CribblyBackend.Test.Support.Games.Repositories;
using CribblyBackend.Test.Support.Teams.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CribblyBackend.Core.UnitTests.Standings.Services
{
    public class StandingsServiceTests
    {
        private readonly IStandingsService _standingsService;
        private readonly FakeGameRepository _fakeGameRepository;
        private readonly FakeTeamRepository _fakeTeamRepository;

        public StandingsServiceTests()
        {
            var provider = ServiceProviderFactory.GetProvider();
            _fakeGameRepository = provider.GetRequiredService<FakeGameRepository>();
            _fakeTeamRepository = provider.GetRequiredService<FakeTeamRepository>();
            _standingsService = provider.GetRequiredService<IStandingsService>();
        }
        [Fact]
        public async Task Calculate_ShouldWork()
        {
            var teams = TestData.CreateTeams(3).ToList();
            foreach (var t in teams)
            {
                t.Id = await _fakeTeamRepository.CreateAsync(t);
            }

            var games = new List<Game>
            {
                // team 1's play in games (2 wins, 1 loss, 1 no winner yet, 222 points)
                new() {Teams = new() {teams[0], teams[1]}, GameRound = Round.Round1, Winner = null},
                new() {Teams = new() {teams[0], teams[1]}, GameRound = Round.Round2, Winner = teams[0], ScoreDifference = 10}, // 121
                new() {Teams = new() {teams[0], teams[2]}, GameRound = Round.Round3, Winner = teams[1], ScoreDifference = 20}, // 101
                new() {Teams = new() {teams[0], teams[2]}, GameRound = Round.Round2, Winner = teams[0], ScoreDifference = 30}, // 121
                // another random play in game
                new() {Teams = new() {teams[1], teams[2]}, GameRound = Round.Round3, Winner = teams[1], ScoreDifference = 69},
                // team 1's tournament games (3 wins, 2 losses, 1 no winner yet, 273 points)
                new() {Teams = new() {teams[0], teams[2]}, GameRound = Round.TourneyRound1, Winner = teams[0], ScoreDifference = 10}, // 121
                new() {Teams = new() {teams[0], teams[2]}, GameRound = Round.SemiFinal, Winner = teams[0], ScoreDifference = 20}, // 121
                new() {Teams = new() {teams[0], teams[2]}, GameRound = Round.QuarterFinal, Winner = teams[0], ScoreDifference = 30}, // 121
                new() {Teams = new() {teams[0], teams[2]}, GameRound = Round.SemiFinal, Winner = teams[2], ScoreDifference = 40}, // 81
                new() {Teams = new() {teams[0], teams[2]}, GameRound = Round.Final, Winner = teams[2], ScoreDifference = 50}, // 71
                new() {Teams = new() {teams[0], teams[2]}, GameRound = Round.Final, Winner = null},
                // another random tournament game
                new() {Teams = new() {teams[1], teams[2]}, GameRound = Round.Final, Winner = teams[2], ScoreDifference = 50},
            };
            foreach (var g in games)
            {
                await _fakeGameRepository.CreateAsync(g);
            }

            var result = await _standingsService.Calculate(teams[0]);
            Assert.Equal(4, result.PlayInGames.Count);
            Assert.Equal(6, result.BracketGames.Count);
            Assert.Equal(5, result.Wins);
            Assert.Equal(3, result.Losses);
            Assert.Equal(121 + 101 + 121 + 121 + 121 + 121 + 81 + 71, result.TotalScore);
        }
    }
}