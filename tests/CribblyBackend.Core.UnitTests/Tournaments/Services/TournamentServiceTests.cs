using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Models;
using CribblyBackend.Core.Tournaments.Repositories;
using CribblyBackend.Core.Tournaments.Services;
using Moq;
using Xunit;

namespace CribblyBackend.Core.UnitTests.Tournaments.Services
{
    public class TournamentServiceTests
    {
        private readonly ITournamentService tournamentService;
        private readonly Mock<ITournamentRepository> mockTournamentRepository;

        public TournamentServiceTests()
        {
            mockTournamentRepository = new Mock<ITournamentRepository>();
            tournamentService = new TournamentService(mockTournamentRepository.Object);
        }

        [Fact]
        public async Task GetNextTournament_ShouldThrowError_WhenQueryFindsTwoTournamentsOpenForRegistration()
        {
            var mockTournaments = new List<Tournament>()
            {
                new Tournament() { Date = new DateTime(1992, 3, 10) },
                new Tournament() { Date = new DateTime(1995, 4, 14) },
            };
            mockTournamentRepository
                .Setup(x => x.GetTournamentsWithActiveFlag(It.IsAny<string>()))
                .ReturnsAsync(mockTournaments);

            await Assert.ThrowsAsync<Exception>(async () => await tournamentService.GetNextTournament());
        }

        [Fact]
        public async Task ChangeActiveStatus_ShouldUpdateDatabase_WhenCanActivateTournament()
        {
            var tournamentId = 123;
            SetupMockTournamentRepositoryFlagCalls(new List<Tournament> { });

            await tournamentService.ChangeActiveStatus(tournamentId, true);

            VerifyMockTournamentRepositoryFlagCallsMadeWith(tournamentId, "IsActive", true);
        }

        [Fact]
        public async Task ChangeActiveStatus_ShouldUpdateDatabase_WhenCanDeactivateTournament()
        {
            var tournamentId = 123;
            SetupMockTournamentRepositoryFlagCalls(
                new List<Tournament> { new Tournament { IsActive = true } }
            );

            await tournamentService.ChangeActiveStatus(tournamentId, false);

            VerifyMockTournamentRepositoryFlagCallsMadeWith(tournamentId, "IsActive", false);
        }

        [Theory]
        [MemberData(nameof(ChangeActiveStatus_ErrorTests_Data))]
        public async Task ChangeActiveStatus_ShouldThrowErrors(bool newVal, List<Tournament> mockQueryResult)
        {
            var tournamentId = 123;
            SetupMockTournamentRepositoryFlagCalls(mockQueryResult);

            await Assert.ThrowsAsync<Exception>(async () => await tournamentService.ChangeActiveStatus(tournamentId, newVal));
        }

        public static IEnumerable<object[]> ChangeActiveStatus_ErrorTests_Data =>
            new List<object[]>
            {
                // trying to activate a tourney when one already is active
                new object[] { true, new List<Tournament> { new Tournament { } } },
                // trying to deactivate a tourney when none are active
                new object[] { false, new List<Tournament> { new Tournament { } } },
                // trying to deactivate a tourney when more than one are active
                new object[] { false, new List<Tournament> { new Tournament { }, new Tournament { } } },
                // trying to deactivate a tourney when the returned tourney isn't active
                new object[] { false, new List<Tournament> { new Tournament { IsActive = false } } },
            };

        [Fact]
        public async Task ChangeOpenForRegistrationStatus_ShouldUpdateDatabase_WhenCanOpenTournament()
        {
            var tournamentId = 123;
            SetupMockTournamentRepositoryFlagCalls(new List<Tournament> { });

            await tournamentService.ChangeOpenForRegistrationStatus(tournamentId, true);

            VerifyMockTournamentRepositoryFlagCallsMadeWith(tournamentId, "IsOpenForRegistration", true);
        }


        [Fact]
        public async Task ChangeOpenForRegistrationStatus_ShouldUpdateDatabase_WhenCanCloseTournament()
        {
            var tournamentId = 123;
            SetupMockTournamentRepositoryFlagCalls(
                new List<Tournament> { new Tournament { IsOpenForRegistration = true } }
            );

            await tournamentService.ChangeOpenForRegistrationStatus(tournamentId, false);

            VerifyMockTournamentRepositoryFlagCallsMadeWith(tournamentId, "IsOpenForRegistration", false);
        }

        [Theory]
        [MemberData(nameof(ChangeOpenForRegistration_ErrorTests_Data))]
        public async Task ChangeOpenForRegistrationStatus_ShouldThrowErrors(bool newVal, List<Tournament> mockQueryResult)
        {
            var tournamentId = 123;
            SetupMockTournamentRepositoryFlagCalls(mockQueryResult);

            await Assert.ThrowsAsync<Exception>(async () => await tournamentService.ChangeOpenForRegistrationStatus(tournamentId, newVal));
        }

        public static IEnumerable<object[]> ChangeOpenForRegistration_ErrorTests_Data =>
            new List<object[]>
            {
                // trying to open a tourney when one already is open
                new object[] { true, new List<Tournament> { new Tournament { } } },
                // trying to close a tourney when none are open
                new object[] { false, new List<Tournament> { new Tournament { } } },
                // trying to close a tourney when more than one are open
                new object[] { false, new List<Tournament> { new Tournament { }, new Tournament { } } },
                // trying to close a tourney when the returned tourney isn't open
                new object[] { false, new List<Tournament> { new Tournament { IsOpenForRegistration = false } } },
            };

        private void SetupMockTournamentRepositoryFlagCalls(List<Tournament> fetchResult)
        {
            mockTournamentRepository
                .Setup(r => r.GetTournamentsWithActiveFlag(It.IsAny<string>()))
                .ReturnsAsync(fetchResult);
            mockTournamentRepository
                .Setup(r => r.SetFlagValue(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()));
        }

        private void VerifyMockTournamentRepositoryFlagCallsMadeWith(int expTourneyId, string expFlagName, bool expVal)
        {
            mockTournamentRepository.Verify(r => r.GetTournamentsWithActiveFlag(
                It.Is<string>(s => s == expFlagName)
            ));
            mockTournamentRepository.Verify(r => r.SetFlagValue(
                It.Is<int>(n => n == expTourneyId),
                It.Is<string>(s => s == expFlagName),
                It.Is<bool>(b => b == expVal)
            ));
        }
    }
}