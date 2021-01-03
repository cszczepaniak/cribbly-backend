using System;
using System.Threading.Tasks;
using CribblyBackend.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService GameService;
        public GameController(IGameService GameService)
        {
            this.GameService = GameService;
        }

        /// <summary>
        /// GetById fetches the specified Game.
        /// </summary>
        /// <param name="id">The id of the Game to get</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await GameService.GetById(id);
            if (p != null)
            {
                return Ok(p);
            }
            return NotFound();
        }
        
        /// <summary>
        /// Create makes a new Game object.
        /// </summary>
        /// <param name="Game">The Game object that will be created</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Game Game)
        {
            try
            {
                await GameService.Create(Game);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
            return Ok();
        }
    }
}