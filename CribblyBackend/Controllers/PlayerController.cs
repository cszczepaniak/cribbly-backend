using System;
using System.Threading.Tasks;
using CribblyBackend.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService playerService;
        public PlayerController(IPlayerService playerService)
        {
            this.playerService = playerService;
        }

        /// <summary>
        /// GetById returns a player given an ID.
        /// </summary>
        /// <param name="id">The ID of the player to get</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await playerService.GetById(id);
            if (p != null)
            {
                return Ok(p);
            }
            return NotFound();
        }

        /// <summary>
        /// GetByEmail returns a player given an email address. The email address must be passed in the custom `Email` header.
        /// This is to avoid exposing personal information in a url.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetByEmail()
        {
            var emailHeaderExists = Request.Headers.TryGetValue("Email", out StringValues email);
            if (!emailHeaderExists)
            {
                return BadRequest("`Email` header must be provided");
            }
            var p = await playerService.GetByEmail(email);
            if (p != null)
            {
                return Ok(p);
            }
            return NotFound();
        }

        /// <summary>
        /// Create creates the specified player.
        /// </summary>
        /// <param name="player">The player to create</param>
        /// <returns></returns>
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