using System;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Exceptions;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Divisions.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DivisionController : ControllerBase
    {
        private readonly IDivisionService _divisionService;
        private readonly ILogger _logger;
        public DivisionController(IDivisionService divisionService, ILogger logger)
        {
            _divisionService = divisionService;
            _logger = logger;
        }

        /// <summary>
        /// GetById returns the Division object that matches the given Id.
        /// </summary>
        /// <param name="Id">The Id of the Division to get</param>
        /// <returns>Division</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.Information("Received request to get division {@id}", id);
                var division = await _divisionService.GetById(id);
                if (division == null)
                {
                    return NotFound();
                }
                return Ok(division);
            }
            catch (Exception e)
            {
                _logger.Information(e.Message, "Failed to get Division: {@Division}", id);
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
        }

        /// <summary>
        /// Create makes a new Division object.
        /// </summary>
        /// <param name="Division">The Division object that will be created</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Division division)
        {
            try
            {
                _logger.Information("Received request to create Division: {@Division}", division);
                var created = await _divisionService.Create(division);
                return Ok(created);
            }
            catch (Exception e)
            {
                _logger.Information(e.Message, "Failed to create Division: {@Division}", division);
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
        }

        /// <summary>
        /// Adds a Team to the specified division.
        /// </summary>
        /// <param name="Division">The Id of the division that the Team will be added to</param>
        /// <param name="Team">The Team that will be added to the Division</param>
        /// <returns></returns>
        [HttpPatch("{id}/team")]
        public async Task<IActionResult> AddTeam(int id, [FromBody] Team team)
        {
            try
            {
                _logger.Information("Received request to add team {@team} to division {@div}", team, id);
                Division div = await _divisionService.AddTeam(id, team);
                return Ok(div);
            }
            catch (DivisionNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.Information(e, "Failed to add team {@team} to division {@div}", team, id);
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
        }
    }
}