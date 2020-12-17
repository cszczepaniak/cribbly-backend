using System;
using System.Threading.Tasks;
using CribblyBackend.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService playerService;
        public PlayerController(IPlayerService playerService)
        {
            this.playerService = playerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var p = await playerService.GetByEmail(email);
            if (p != null)
            {
                return Ok(await playerService.GetByEmail(email));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Player player)
        {
            try
            {
                await playerService.Create(player);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
            return Ok();
        }
    }
}