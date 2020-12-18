using System;
using System.Threading.Tasks;
using CribblyBackend.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService TeamService;
        public TeamController(ITeamService TeamService)
        {
            this.TeamService = TeamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var p = await TeamService.GetById(id);
            if (p != null)
            {
                return Ok(await TeamService.GetById(id));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Team Team)
        {
            try
            {
                await TeamService.Create(Team);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
            return Ok();
        }
    }
}