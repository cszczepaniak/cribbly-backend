using System;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.Network;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Serilog;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StandingsController : ControllerBase
    {
        private readonly IStandingsService StandingsService;
        private readonly ILogger logger;
        public StandingsController(IStandingsService StandingsService, ILogger logger)
        {
            this.StandingsService = StandingsService;
            this.logger = logger;
        }

        /// <summary>
        /// Returns a Team object with standings filled in.
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Get([FromBody] Team team)
        {
            try
            {
                var standing = await StandingsService.Calculate(team);
                return Ok(standing);
            }
            catch (Exception ex)
            {
                logger.Information(ex, "Failed to get standings");
                return StatusCode(500, $"Failed to get standings: {ex.Message}");
            }

        }
    }
}