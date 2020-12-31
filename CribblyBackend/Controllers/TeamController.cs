using System;
using System.Threading.Tasks;
using CribblyBackend.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService teamService;
        public TeamController(ITeamService teamService)
        {
            this.teamService = teamService;
        }

        /// <summary>
        /// GetById fetches the specified team.
        /// </summary>
        /// <param name="id">The id of the team to get</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await teamService.GetById(id);
            if (p != null)
            {
                return Ok(p);
            }
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
                await teamService.Create(team);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
            return Ok();
        }
    }
}