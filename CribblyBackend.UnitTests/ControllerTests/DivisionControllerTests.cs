using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CribblyBackend.Controllers;
using CribblyBackend.DataAccess.Exceptions;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Divisions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class DivisionControllerTests
    {
        private readonly Mock<HttpRequest> mockHttpRequest;
        private readonly Mock<HttpContext> mockHttpContext;
        private readonly DivisionController divisionController;
        private readonly Mock<IDivisionService> mockDivisionService;
        private readonly Mock<ILogger> mockLoggerService;
        private Team _team1 = new Team() { Id = 1, Name = "team1"};
        private Team _team2 = new Team() { Id = 2, Name = "team2"};
        private Division _division = new Division() { Id = 1, Name = "Test", Teams = new List<Team>()};


        public DivisionControllerTests()
        {
            _division.Teams.Add(_team1);
            _division.Teams.Add(_team2);
            mockDivisionService = new Mock<IDivisionService>();
            mockLoggerService = new Mock<ILogger>();
            mockHttpRequest = new Mock<HttpRequest>();
            mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            divisionController = new DivisionController(mockDivisionService.Object, mockLoggerService.Object);
            divisionController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task Create_ShouldReturnOk_IfNoError()
        {
            Division division = new Division() { Id = 1 };
            mockDivisionService.Setup(x => x.Create(It.IsAny<Division>())).ReturnsAsync(division);
            var result = await divisionController.Create(division);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturn500_IfError()
        {
            mockDivisionService.Setup(x => x.Create(It.IsAny<Division>())).Throws(new Exception());
            var result = await divisionController.Create(new Division());
            var typedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, typedResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_IfNoError()
        {
            mockDivisionService.Setup(x => x.GetById(1)).ReturnsAsync(_division);
            var result = await divisionController.GetById(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ShouldReturn404_IfDivisionNotFound()
        {
            mockDivisionService.Setup(x => x.GetById(1)).Throws(new DivisionNotFoundException("Test"));
            var result = await divisionController.GetById(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddTeam_ShouldReturnOk_IfNoError()
        {
            _division.Teams.Remove(_team2);
            mockDivisionService.Setup(x => x.AddTeam(1, It.IsAny<Team>())).ReturnsAsync(_division);
            var result = await divisionController.AddTeam(1, _team2);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddTeam_ShouldReturn404_IfDivisionNotFound()
        {
            mockDivisionService.Setup(x => x.AddTeam(1, It.IsAny<Team>())).Throws(new DivisionNotFoundException("Test"));
            var result = await divisionController.AddTeam(1, _team1);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}