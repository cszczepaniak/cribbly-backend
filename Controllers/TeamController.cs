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
        public TeamController(ITeamService TeamService)
        {
            this.teamService = TeamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var p = await teamService.GetById(id);
            if (p != null)
            {
                return Ok(p);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Team Team)
        {
            try
            {
                await teamService.Create(Team);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
            return Ok();
        }
    }
}