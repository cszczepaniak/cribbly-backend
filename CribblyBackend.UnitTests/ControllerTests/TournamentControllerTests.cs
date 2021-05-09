using System;
using CribblyBackend.Controllers;
using CribblyBackend.Core.Tournaments.Models;
using CribblyBackend.Network;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cribbly.UnitTessts
{
    public class TournamentControllerTests
    {
        private readonly Mock<ITournamentService> mockTournamentService;
        private readonly TournamentController tournamentController;
        public TournamentControllerTests()
        {
            mockTournamentService = new Mock<ITournamentService>();
            tournamentController = new TournamentController(mockTournamentService.Object);
        }

        [Fact]
        public async void GetNextTournament_ShouldReturnNotFound_IfNoTournamentFound()
        {
            mockTournamentService.Setup(x => x.GetNextTournament()).ReturnsAsync((Tournament)null);

            var result = Assert.IsType<NotFoundResult>(await tournamentController.GetNextTournament());
        }

        [Fact]
        public async void GetNextTournament_ShouldReturnOkAndTournament_IfTournamentFound()
        {
            var testTournament = new Tournament()
            {
                Id = 123
            };
            mockTournamentService.Setup(x => x.GetNextTournament()).ReturnsAsync(testTournament);

            var result = Assert.IsType<OkObjectResult>(await tournamentController.GetNextTournament());
            var tournament = Assert.IsType<Tournament>(result.Value);
            Assert.Equal(testTournament.Id, tournament.Id);
        }

        [Fact]
        public async void Create_ShouldReturnOk_IfTournamentCreated()
        {
            var testDate = new DateTime(2020, 1, 1);
            var request = new CreateTournamentRequest() { Date = testDate };
            mockTournamentService.Setup(x => x.Create(testDate));

            var result = Assert.IsType<OkResult>(await tournamentController.CreateTournament(request));
            mockTournamentService.VerifyAll();
        }

        [Fact]
        public async void Create_ShouldReturnError_IfTournamentCreateFails()
        {
            var testDate = new DateTime(2020, 1, 1);
            var request = new CreateTournamentRequest() { Date = testDate };
            mockTournamentService.Setup(x => x.Create(It.IsAny<DateTime>())).ThrowsAsync(new Exception("Bad time"));

            var result = Assert.IsType<ObjectResult>(await tournamentController.CreateTournament(request));
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Fact]
        public async void ChangeTournamentFlags_ReturnsBadRequest_IfNoFlagsInRequest()
        {
            var request = new ChangeTournamentFlagsRequest() { Id = 123 };

            var result = Assert.IsType<BadRequestObjectResult>(await tournamentController.ChangeTournamentFlags(request));
        }

        [Fact]
        public async void ChangeTournamentFlags_ChangesFlags_AndReturnsOk()
        {
            var isActive = true;
            var isOpenForRegistration = false;
            var request = new ChangeTournamentFlagsRequest()
            {
                Id = 123,
                IsActive = isActive,
                IsOpenForRegistration = isOpenForRegistration,
            };
            mockTournamentService.Setup(x => x.ChangeActiveStatus(request.Id, isActive));
            mockTournamentService.Setup(x => x.ChangeOpenForRegistrationStatus(request.Id, isOpenForRegistration));

            var result = Assert.IsType<OkResult>(await tournamentController.ChangeTournamentFlags(request));
            mockTournamentService.VerifyAll();
        }

        [Fact]
        public async void ChangeTournamentFlags_DoesNotCallChangeActiveStatus_IfIsActiveIsNull()
        {
            bool? isActive = null;
            var isOpenForRegistration = false;
            var request = new ChangeTournamentFlagsRequest()
            {
                Id = 123,
                IsActive = isActive,
                IsOpenForRegistration = isOpenForRegistration,
            };
            mockTournamentService.Setup(x => x.ChangeActiveStatus(request.Id, It.IsAny<bool>()));
            mockTournamentService.Setup(x => x.ChangeOpenForRegistrationStatus(request.Id, isOpenForRegistration));

            await tournamentController.ChangeTournamentFlags(request);

            mockTournamentService.Verify(x => x.ChangeActiveStatus(It.IsAny<int>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public async void ChangeTournamentFlags_DoesNotCallChangeOpenForRegistrationStatus_IfIsOpenForRegistrationIsNull()
        {
            var isActive = false;
            bool? isOpenForRegistration = null;
            var request = new ChangeTournamentFlagsRequest()
            {
                Id = 123,
                IsActive = isActive,
                IsOpenForRegistration = isOpenForRegistration,
            };
            mockTournamentService.Setup(x => x.ChangeActiveStatus(request.Id, isActive));
            mockTournamentService.Setup(x => x.ChangeOpenForRegistrationStatus(request.Id, It.IsAny<bool>()));

            await tournamentController.ChangeTournamentFlags(request);

            mockTournamentService.Verify(x => x.ChangeOpenForRegistrationStatus(It.IsAny<int>(), It.IsAny<bool>()), Times.Never());
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(null, true)]
        [InlineData(true, null)]
        public async void ChangeTournamentFlags_ReturnsError_IfCallToChangeFlagsThrows(
            bool? isActive,
            bool? isOpenForRegistration)
        {
            var request = new ChangeTournamentFlagsRequest()
            {
                Id = 123,
                IsActive = isActive,
                IsOpenForRegistration = isOpenForRegistration
            };
            mockTournamentService.Setup(x => x.ChangeActiveStatus(request.Id, It.IsAny<bool>())).ThrowsAsync(new Exception());
            mockTournamentService.Setup(x => x.ChangeOpenForRegistrationStatus(request.Id, It.IsAny<bool>())).ThrowsAsync(new Exception());

            var result = Assert.IsType<ObjectResult>(await tournamentController.ChangeTournamentFlags(request));
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }
    }
}