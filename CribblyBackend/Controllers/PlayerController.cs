using System;
using System.Threading.Tasks;
using CribblyBackend.Models;
using CribblyBackend.Models.Network;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Http;
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
        /// Handles the login. Creates or gets the player specified in the request.
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<LoginResponse> Login([FromBody] LoginRequest request)
        {
            var exists = await playerService.Exists(request.Email);
            Player player;
            if (exists)
            {
                player = await playerService.GetByEmail(request.Email);
                return new LoginResponse()
                {
                    Player = player,
                    IsReturning = true,
                };
            }
            player = await playerService.Create(request.Email, request.Name);
            return new LoginResponse()
            {
                Player = player,
                IsReturning = false,
            };
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
                await playerService.Create(player.Email, player.Name);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e.Message}");
            }
            return Ok();
        }
    }
}