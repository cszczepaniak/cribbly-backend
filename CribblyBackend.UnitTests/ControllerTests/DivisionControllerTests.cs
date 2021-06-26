using System;
using System.Threading.Tasks;
using CribblyBackend.Controllers;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.Services;
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


        public DivisionControllerTests()
        {
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
        public async Task Create_ShouldReturnOkAndCreatedId_IfNoError()
        {
            Division division = new Division();
            mockDivisionService.Setup(x => x.Create(It.IsAny<Division>())).ReturnsAsync(division);
            var result = await divisionController.Create(new Division());
            var typedResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<Division>(typedResult.Value);
        }

        [Fact]
        public async Task Create_ShouldReturn500_IfError()
        {
            mockDivisionService.Setup(x => x.Create(It.IsAny<Division>())).Throws(new Exception());
            var result = await divisionController.Create(new Division());
            var typedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, typedResult.StatusCode);
        }
    }
}