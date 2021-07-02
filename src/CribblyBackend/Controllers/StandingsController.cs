using System;
using System.Threading.Tasks;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Teams.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StandingsController : ControllerBase
    {
        private readonly IStandingsService _standingsService;
        private readonly ILogger _logger;
        public StandingsController(IStandingsService standingsService, ILogger logger)
        {
            _standingsService = standingsService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a Team object with standings filled in.
        /// </summary>
        /// <param name="team">The team for which to get standings</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Calculate")]
        public async Task<IActionResult> Calculate([FromBody] Team team)
        {
            try
            {
                var standing = await _standingsService.Calculate(team);
                return Ok(standing);
            }
            catch (Exception ex)
            {
                _logger.Information(ex, "Failed to get standings");
                return StatusCode(500, $"Failed to get standings: {ex.Message}");
            }

        }
    }
}