using System;
using System.Net;
using System.Net.Http.Json;
using CribblyBackend.Api.Tests.Common;
using CribblyBackend.Core.Tournaments.Models;
using CribblyBackend.Network;
using CribblyBackend.Test.Support.Tournaments.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CribblyBackend.Api.Tests
{
    public class TournamentControllerTests : ApiTestBase
    {
        private readonly FakeTournamentRepository _fakeTournamentRepository;
        public TournamentControllerTests()
        {
            _fakeTournamentRepository = _factory.Services.GetRequiredService<FakeTournamentRepository>();
        }

        [Fact]
        public async void GetNextTournament_ShouldReturnNotFound_IfNoTournamentFound()
        {
            var result = await _factory.CreateClient().GetAsync("/api/tournament/next");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async void GetNextTournament_ShouldReturnOkAndTournament_IfTournamentFound()
        {
            var tournament = await _fakeTournamentRepository.CreateAsync(DateTime.Now);
            await _fakeTournamentRepository.SetFlagValueAsync(tournament.Id, nameof(tournament.IsOpenForRegistration), true);

            var result = await _factory.CreateClient().GetAsync("/api/tournament/next");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var actualTournament = await result.Content.ReadFromJsonAsync<Tournament>();
            Assert.Equal(tournament.Id, actualTournament.Id);
            Assert.Equal(tournament.Date, actualTournament.Date);
        }

        [Fact]
        public async void Create_ShouldReturnOk_IfTournamentCreated()
        {
            var testDate = new DateTime(2020, 1, 1);
            var request = new CreateTournamentRequest() { Date = testDate };
            var result = await _factory.CreateClient().PostAsJsonAsync("/api/tournament", request);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async void ChangeTournamentFlags_ReturnsBadRequest_IfNoFlagsInRequest()
        {
            var request = new ChangeTournamentFlagsRequest() { Id = 123 };

            var result = await _factory.CreateClient().PostAsJsonAsync("/api/tournament/setFlags", request);

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async void ChangeTournamentFlags_ChangesFlags_AndReturnsOk()
        {
            var tournament = await _fakeTournamentRepository.CreateAsync(DateTime.Now);
            Assert.False(tournament.IsActive);
            Assert.False(tournament.IsOpenForRegistration);

            var request = new ChangeTournamentFlagsRequest
            {
                Id = tournament.Id,
                IsActive = true,
                IsOpenForRegistration = true,
            };

            var result = await _factory.CreateClient().PostAsJsonAsync("/api/tournament/setFlags", request);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            tournament = await _fakeTournamentRepository.GetByIdAsync(tournament.Id);
            Assert.True(tournament.IsActive);
            Assert.True(tournament.IsOpenForRegistration);
        }
    }
}