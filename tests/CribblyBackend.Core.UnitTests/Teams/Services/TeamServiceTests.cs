using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Teams.Services;
using CribblyBackend.Test.Support;
using CribblyBackend.Test.Support.Teams.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace CribblyBackend.Core.UnitTests.Teams.Services
{
    public class TeamServiceTests
    {
        private readonly ITeamService _teamService;
        private readonly FakeTeamRepository _fakeTeamRepository;

        public TeamServiceTests()
        {
            var provider = ServiceProviderFactory.GetProvider();
            _fakeTeamRepository = provider.GetRequiredService<FakeTeamRepository>();
            _teamService = provider.GetRequiredService<ITeamService>();
        }
        [Fact]
        public async Task CreateTeamWithOnePlayer_ShouldThrowError()
        {
            var badTeam = new Team()
            {
                Name = "bad",
                Players = new List<Player>() { new Player() { Name = "Tobias Funke" } }
            };
            await Assert.ThrowsAsync<Exception>(async () => await _teamService.Create(badTeam));
        }

        [Fact]
        public async Task Get_ShouldNotReturnDuplicateTeams()
        {
            var players = new List<Player>();
            for (int i = 0; i < 4; i++)
            {
                players.Add(new() { Id = i });
            }
            await _fakeTeamRepository.CreateAsync(new Team() { Name = "team1", Players = players.Take(2).ToList() });
            await _fakeTeamRepository.CreateAsync(new Team() { Name = "team2", Players = players.Take(2).ToList() });

            var teamsList = await _teamService.Get();
            Assert.Collection(teamsList,
                t => Assert.Equal(1, t.Id),
                t => Assert.Equal(2, t.Id)
            );
        }
    }
}