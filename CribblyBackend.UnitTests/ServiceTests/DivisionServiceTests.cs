using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;
using CribblyBackend.Services;
using Moq;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class DivisionServiceTests
    {
        private readonly DivisionService _divisionService;
        private readonly Mock<IDivisionRepository> _mockDivisionRepository;

        public DivisionServiceTests()
        {
            _mockDivisionRepository = new Mock<IDivisionRepository>();
            _divisionService = new DivisionService(_mockDivisionRepository.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedObject()
        {
            Division division = new Division() {Id = 1, Name = "Test"};
            _mockDivisionRepository.Setup(x => x.Create(It.IsAny<Division>())).ReturnsAsync(division);
            var result = await _divisionService.Create(division);
            Assert.Equal(division, result);
        }
    }
}