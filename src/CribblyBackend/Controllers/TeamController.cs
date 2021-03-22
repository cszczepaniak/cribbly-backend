using System;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;


namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService teamService;
        private readonly ILogger logger;
        public TeamController(ITeamService teamService, ILogger logger)
        {
            this.teamService = teamService;
            this.logger = logger;
        }

        /// <summary>
        /// GetById fetches the specified team.
        /// </summary>
        /// <param name="id">The id of the team to get</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            logger.Debug("Received request to get team using id {id}", id);
            var p = await teamService.GetById(id);
            if (p != null)
            {
                return Ok(p);
            }
            logger.Information("Request for team {id} returned no results", id);
            return NotFound();
        }

        /// <summary>
        /// Create makes a new Team object, and updates Player records accordingly in the database.
        /// </summary>
        /// <param name="Team">The Team object that will be created</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Team team)
        {
            try
            {
                logger.Debug("Received request to create team {@team}", team);
                var createdId = await teamService.Create(team);
                logger.Debug("New team created: {@team}", team);
                return Ok(createdId);
            }
            catch (Exception e)
            {
                logger.Information("Failed to create team: {@team} -- MSG: {message}", team, e.Message);
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
        }
        /// <summary>
        /// GetAll returns all teams that are in the active tournament. 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Get()
        {
            try
            {
                logger.Debug("Received request to get all teams");
                var teams = await teamService.GetAll();
                logger.Debug("All teams returned");
                return Ok(teams);
            }
            catch (Exception e)
            {
                logger.Information(e, "Failed to fetch all teams");
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
        }
    }
}