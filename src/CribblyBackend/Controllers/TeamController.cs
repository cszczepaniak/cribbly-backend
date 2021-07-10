using System;
using System.Linq;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Exceptions;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;


namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ILogger _logger;
        public TeamController(ITeamService teamService, ILogger logger)
        {
            _teamService = teamService;
            _logger = logger;
        }

        /// <summary>
        /// GetById fetches the specified team.
        /// </summary>
        /// <param name="id">The id of the team to get</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.Debug("Received request to get team using id {id}", id);
            var p = await _teamService.GetById(id);
            if (p != null)
            {
                return Ok(p);
            }
            _logger.Information("Request for team {id} returned no results", id);
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
                _logger.Debug("Received request to create team {@team}", team);
                var createdId = await _teamService.Create(team);
                _logger.Debug("New team created: {@team}", team);
                return Ok(createdId);
            }
            catch (Exception e)
            {
                _logger.Information("Failed to create team: {@team} -- MSG: {message}", team, e.Message);
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
        }
        /// <summary>
        /// GetAll returns all teams that are in the active tournament. 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.Debug("Received request to get all teams");
                var teams = await _teamService.Get();
                _logger.Debug("All teams returned");
                return Ok(teams);
            }
            catch (Exception e)
            {
                _logger.Information(e, "Failed to fetch all teams");
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
        }

        /// <summary>
        /// GetByTeamId fetches all games associated with a given TeamId.
        /// </summary>
        /// <param name="id">The id of the Team for which to get all games</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/games")]
        public async Task<IActionResult> GetByTeamId(int id)
        {
            var games = await _teamService.GetGamesAsync(id);
            if (games.Any())
            {
                return Ok(games);
            }
            _logger.Information("Request for games from team {id} returned no results", id);
            return NotFound();
        }

        /// <summary>
        /// Delete permanently removes the team with the specified id.
        /// </summary>
        /// <param name="id">The Team object that will be deleted from the database</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.Information("Received request to delete team {@id}", id);

            try
            {
                await _teamService.Delete(id); 
                _logger.Warning("Team {@id} was deleted", id);
                return NoContent();
            }
            catch (TeamNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.Information(e, "Failed to delete {@id}", id);
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }

        }
    }
}