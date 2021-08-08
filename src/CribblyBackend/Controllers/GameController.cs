using System;
using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Games.Services;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger _logger;
        public GameController(IGameService gameService, ILogger logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        /// <summary>
        /// GetById fetches the specified Game.
        /// </summary>
        /// <param name="id">The id of the Game to get</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _gameService.GetById(id);
            if (p != null)
            {
                return Ok(p);
            }
            _logger.Information("Request for game {id} returned no results", id);
            return NotFound();
        }

        /// <summary>
        /// GetByTeamId fetches all games associated with a given TeamId.
        /// </summary>
        /// <param name="id">The id of the Team for which to get all games</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery(Name = "team")] int? teamId)
        {
            if (!teamId.HasValue)
            {
                return BadRequest("Must provide a team ID as a query parameter");
            }
            var games = await _gameService.GetByTeamAsync(teamId.Value);
            if (games.Any())
            {
                return Ok(games);
            }
            _logger.Information("Request for games from team {id} returned no results", teamId);
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
                _logger.Information("Received request to create game: {@game}", game);
                game = await _gameService.Create(game);
                return Ok(game);
            }
            catch (Exception e)
            {
                _logger.Information(e.Message, "Failed to create game: {@game}", game);
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
        }

        /// <summary>
        /// Update changes a given Game object based upon a version of the Game included in the request body.
        /// </summary>
        /// <param name="Game">The Game object that will be updated</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] Game game)
        {
            try
            {
                _logger.Information("Received request to update game: {@game}", game);
                Game updatedGame = await _gameService.Update(game);
                if (updatedGame != null)
                {
                    return Ok(updatedGame);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
        }
    }
}