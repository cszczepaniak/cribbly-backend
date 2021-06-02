using System;
using System.Threading.Tasks;
using System.Linq;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService gameService;
        private readonly ILogger logger;
        public GameController(IGameService gameService, ILogger logger)
        {
            this.gameService = gameService;
            this.logger = logger;
        }

        /// <summary>
        /// GetById fetches the specified Game.
        /// </summary>
        /// <param name="id">The id of the Game to get</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await gameService.GetById(id);
            if (p != null)
            {
                return Ok(p);
            }
            logger.Information("Request for game {id} returned no results", id);
            return NotFound();
        }

        /// <summary>
        /// Create makes a new Game object.
        /// </summary>
        /// <param name="Game">The Game object that will be created</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Game game)
        {
            try
            {
                logger.Information("Received request to create game: {@game}", game);
                await gameService.Create(game);
            }
            catch (Exception e)
            {
                logger.Information("Failed to create game: {@game} -- MSG: {message}", game, e.Message);
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
            return Ok();
        }
    }
}