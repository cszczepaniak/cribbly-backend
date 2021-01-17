using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CribblyBackend.Controllers;
using CribblyBackend.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Dapper;
using Moq.Dapper;
using System.Data;
using System.Data.Common;

namespace CribblyBackend.UnitTests
{
    public class TournamentServiceTests
    {
        private readonly TournamentService tournamentService;
        private readonly Mock<DbConnection> mockConnection;

        public TournamentServiceTests()
        {
            mockConnection = new Mock<DbConnection>();
            tournamentService = new TournamentService(mockConnection.Object);
        }

        [Fact]
        public async Task GetNextTournament_ShouldReturnNull_WhenNoTournamentsAreOpenForRegistration()
        {
            mockConnection
                .SetupDapperAsync(c => c.QueryAsync<Tournament>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(new List<Tournament>());

            var tournament = await tournamentService.GetNextTournament();
            Assert.Null(tournament);
        }

        [Fact]
        public async Task GetNextTournament_ShouldReturnTournamentOpenForRegistration_WhenSuccessful()
        {
            var activeDate = new DateTime(2021, 3, 1);
            var mockTournaments = new List<Tournament>()
            {
                new Tournament() { Date = activeDate },
            };
            mockConnection
                .SetupDapperAsync(c => c.QueryAsync<Tournament>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(mockTournaments);

            var tournament = await tournamentService.GetNextTournament();
            Assert.Equal(activeDate, tournament.Date);
        }

        [Fact]
        public async Task GetNextTournament_ShouldThrowError_WhenQueryFindsTwoTournamentsOpenForRegistration()
        {
            var mockTournaments = new List<Tournament>()
            {
                new Tournament() { Date = new DateTime(1992, 3, 10) },
                new Tournament() { Date = new DateTime(1995, 4, 14) },
            };
            mockConnection
                .SetupDapperAsync(x => x.QueryAsync<Tournament>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(mockTournaments);

            await Assert.ThrowsAsync<Exception>(async () => await tournamentService.GetNextTournament());
        }

        [Fact]
        public async Task ChangeActiveStatus_ShouldUpdateDatabase_WhenCanActivateTournament()
        {
            var tournamentId = 123;
            mockConnection
                .SetupDapperAsync(
                    x => x.QueryAsync<Tournament>(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(new List<Tournament> { });
            mockConnection
                .SetupDapperAsync(
                    x => x.ExecuteAsync(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(1);

            // Since Moq.Dapper doesn't support Verify* Moq methods, best we can do is test that no error is thrown
            await tournamentService.ChangeActiveStatus(tournamentId, true);
        }

        [Fact]
        public async Task ChangeActiveStatus_ShouldUpdateDatabase_WhenCanDeactivateTournament()
        {
            var tournamentId = 123;
            mockConnection
                .SetupDapperAsync(
                    x => x.QueryAsync<Tournament>(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(new List<Tournament> { new Tournament { IsActive = true } });
            mockConnection
                .SetupDapperAsync(
                    x => x.ExecuteAsync(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(1);

            // Since Moq.Dapper doesn't support Verify* Moq methods, best we can do is test that no error is thrown
            await tournamentService.ChangeActiveStatus(tournamentId, false);
        }

        [Theory]
        [MemberData(nameof(ChangeActiveStatus_ErrorTests_Data))]
        public async Task ChangeActiveStatus_ShouldThrowErrors(bool newVal, List<Tournament> mockQueryResult)
        {
            var tournamentId = 123;
            mockConnection
                .SetupDapperAsync(
                    x => x.QueryAsync<Tournament>(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(mockQueryResult);
            mockConnection
                .SetupDapperAsync(
                    x => x.ExecuteAsync(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(1);

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
            mockConnection
                .SetupDapperAsync(
                    x => x.QueryAsync<Tournament>(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(new List<Tournament> { });
            mockConnection
                .SetupDapperAsync(
                    x => x.ExecuteAsync(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(1);

            // Since Moq.Dapper doesn't support Verify* Moq methods, best we can do is test that no error is thrown
            await tournamentService.ChangeOpenForRegistrationStatus(tournamentId, true);
        }


        [Fact]
        public async Task ChangeOpenForRegistrationStatus_ShouldUpdateDatabase_WhenCanCloseTournament()
        {
            var tournamentId = 123;
            mockConnection
                .SetupDapperAsync(
                    x => x.QueryAsync<Tournament>(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(new List<Tournament> { new Tournament { IsOpenForRegistration = true } });
            mockConnection
                .SetupDapperAsync(
                    x => x.ExecuteAsync(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(1);

            // Since Moq.Dapper doesn't support Verify* Moq methods, best we can do is test that no error is thrown
            await tournamentService.ChangeOpenForRegistrationStatus(tournamentId, false);
        }

        [Theory]
        [MemberData(nameof(ChangeOpenForRegistration_ErrorTests_Data))]
        public async Task ChangeOpenForRegistrationStatus_ShouldThrowErrors(bool newVal, List<Tournament> mockQueryResult)
        {
            var tournamentId = 123;
            mockConnection
                .SetupDapperAsync(
                    x => x.QueryAsync<Tournament>(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(mockQueryResult);
            mockConnection
                .SetupDapperAsync(
                    x => x.ExecuteAsync(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(1);

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

    }
}