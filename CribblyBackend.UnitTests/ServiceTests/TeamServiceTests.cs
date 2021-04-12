using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using CribblyBackend.Controllers;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;
using CribblyBackend.Services;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Dapper;
using Xunit;

namespace CribblyBackend.UnitTests
{
    public class TeamServiceTests
    {
        private readonly TeamService teamService;
        private readonly GameService gameService;
        private readonly Mock<ITeamRepository> mockTeamRepository;
        private readonly Mock<IGameRepository> mockGameRepository;

        public TeamServiceTests()
        {
            mockTeamRepository = new Mock<ITeamRepository>();
            mockGameRepository = new Mock<IGameRepository>();
            gameService = new GameService(mockGameRepository.Object);
            teamService = new TeamService(mockTeamRepository.Object, mockGameRepository.Object);
        }

    }
}