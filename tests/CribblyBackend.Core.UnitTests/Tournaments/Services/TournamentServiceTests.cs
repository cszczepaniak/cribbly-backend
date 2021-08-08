using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Common.Exceptions;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Tournaments.Models;
using CribblyBackend.Core.Tournaments.Services;
using CribblyBackend.Test.Support;
using CribblyBackend.Test.Support.Players.Repositories;
using CribblyBackend.Test.Support.Tournaments.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CribblyBackend.Core.UnitTests.Tournaments.Services
{
    public class TournamentServiceTests
    {
        private readonly ITournamentService _tournamentService;
        private readonly FakePlayerRepository _fakePlayerRepository;
        private readonly FakeTournamentRepository _fakeTournamentRepository;
        private readonly FakeTournamentPlayerRepository _fakeTournamentPlayerRepository;

        public TournamentServiceTests()
        {
            var provider = ServiceProviderFactory.GetProvider();
            _fakePlayerRepository = provider.GetRequiredService<FakePlayerRepository>();
            _fakeTournamentRepository = provider.GetRequiredService<FakeTournamentRepository>();
            _fakeTournamentPlayerRepository = provider.GetRequiredService<FakeTournamentPlayerRepository>();
            _tournamentService = provider.GetRequiredService<ITournamentService>();
        }

        [Fact]
        public async Task GetNextTournament_ShouldThrowError_WhenQueryFindsTwoTournamentsOpenForRegistration()
        {
            var tournaments = new List<Tournament>()
            {
                new Tournament() { Date = new DateTime(1992, 3, 10) },
                new Tournament() { Date = new DateTime(1995, 4, 14) },
            };
            foreach (var t in tournaments)
            {
                var created = await _fakeTournamentRepository.CreateAsync(t.Date);
                await _fakeTournamentRepository.SetFlagValueAsync(created.Id, nameof(created.IsOpenForRegistration), true);
            }

            await Assert.ThrowsAsync<Exception>(async () => await _tournamentService.GetNextTournament());
        }

        [Fact]
        public async Task RegisterPlayer_CreatesAssociation()
        {
            var player = await _fakePlayerRepository.CreateAsync(new() { AuthProviderId = TestData.NewString(), Email = TestData.NewString() });
            var tournament = await _fakeTournamentRepository.CreateAsync(DateTime.Now);
            await _tournamentService.RegisterPlayerAsync(tournament.Id, player.Id);
            Assert.True(_fakeTournamentPlayerRepository.HasAssociation(tournament.Id, player.Id));
        }

        [Fact]
        public async Task RegisterPlayer_Throws_WhenTournamentDoesNotExist()
        {
            var player = await _fakePlayerRepository.CreateAsync(new() { AuthProviderId = TestData.NewString(), Email = TestData.NewString() });
            await Assert.ThrowsAsync<EntityNotFoundException<Tournament>>(() => _tournamentService.RegisterPlayerAsync(1234, player.Id));
        }

        [Fact]
        public async Task RegisterPlayer_Throws_WhenPlayerDoesNotExist()
        {
            var tournament = _fakeTournamentRepository.CreateAsync(DateTime.Now);
            await Assert.ThrowsAsync<EntityNotFoundException<Player>>(() => _tournamentService.RegisterPlayerAsync(tournament.Id, 3456));
        }

        [Fact]
        public async Task ChangeActiveStatus_ShouldWork()
        {
            var tourney1 = await _fakeTournamentRepository.CreateAsync(DateTime.Now);
            var tourney2 = await _fakeTournamentRepository.CreateAsync(DateTime.Now.AddDays(1));

            // changing active status to false for the first tournament should work
            await _tournamentService.ChangeActiveStatus(tourney1.Id, true);
            Assert.True((await _fakeTournamentRepository.GetByIdAsync(tourney1.Id)).IsActive);
            // changing active status to true on a second tournament should NOT work
            await Assert.ThrowsAsync<Exception>(async () => await _tournamentService.ChangeActiveStatus(tourney2.Id, true));
            Assert.False((await _fakeTournamentRepository.GetByIdAsync(tourney2.Id)).IsActive);

            // changing active status to false for the first tournament should work
            await _tournamentService.ChangeActiveStatus(tourney1.Id, false);
            Assert.False((await _fakeTournamentRepository.GetByIdAsync(tourney1.Id)).IsActive);

            // now we should be able to set active status to true on the second one
            await _tournamentService.ChangeActiveStatus(tourney2.Id, true);
            Assert.True((await _fakeTournamentRepository.GetByIdAsync(tourney2.Id)).IsActive);
            await _tournamentService.ChangeActiveStatus(tourney2.Id, false);
            Assert.False((await _fakeTournamentRepository.GetByIdAsync(tourney2.Id)).IsActive);

            // trying to set an already false flag to false should fail
            await Assert.ThrowsAsync<Exception>(async () => await _tournamentService.ChangeActiveStatus(tourney1.Id, false));
            await Assert.ThrowsAsync<Exception>(async () => await _tournamentService.ChangeActiveStatus(tourney2.Id, false));
        }

        [Fact]
        public async Task ChangeOpenForRegistrationStatus_ShouldWork()
        {
            var tourney1 = await _fakeTournamentRepository.CreateAsync(DateTime.Now);
            var tourney2 = await _fakeTournamentRepository.CreateAsync(DateTime.Now.AddDays(1));

            // changing active status to false for the first tournament should work
            await _tournamentService.ChangeOpenForRegistrationStatus(tourney1.Id, true);
            Assert.True((await _fakeTournamentRepository.GetByIdAsync(tourney1.Id)).IsOpenForRegistration);
            // changing active status to true on a second tournament should NOT work
            await Assert.ThrowsAsync<Exception>(async () => await _tournamentService.ChangeOpenForRegistrationStatus(tourney2.Id, true));
            Assert.False((await _fakeTournamentRepository.GetByIdAsync(tourney2.Id)).IsOpenForRegistration);

            // changing active status to false for the first tournament should work
            await _tournamentService.ChangeOpenForRegistrationStatus(tourney1.Id, false);
            Assert.False((await _fakeTournamentRepository.GetByIdAsync(tourney1.Id)).IsOpenForRegistration);

            // now we should be able to set active status to true on the second one
            await _tournamentService.ChangeOpenForRegistrationStatus(tourney2.Id, true);
            Assert.True((await _fakeTournamentRepository.GetByIdAsync(tourney2.Id)).IsOpenForRegistration);
            await _tournamentService.ChangeOpenForRegistrationStatus(tourney2.Id, false);
            Assert.False((await _fakeTournamentRepository.GetByIdAsync(tourney2.Id)).IsOpenForRegistration);

            // trying to set an already false flag to false should fail
            await Assert.ThrowsAsync<Exception>(async () => await _tournamentService.ChangeOpenForRegistrationStatus(tourney1.Id, false));
            await Assert.ThrowsAsync<Exception>(async () => await _tournamentService.ChangeOpenForRegistrationStatus(tourney2.Id, false));
        }
    }
}