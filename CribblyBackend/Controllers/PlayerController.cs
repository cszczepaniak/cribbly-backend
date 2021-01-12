using System;
using System.Threading.Tasks;
using CribblyBackend.Models;
using CribblyBackend.Models.Network;
using CribblyBackend.Services;
using Serilog;
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request.Email == null)
            {
                Log.Information("Login request submitted with no email");
                return BadRequest("Must provide an email");
            }
            var exists = await playerService.Exists(request.Email);
            Player player;
            if (exists)
            {
                player = await playerService.GetByEmail(request.Email);
                Log.Debug($"User {player.Email} successfully logged in");
                return Ok(new LoginResponse()
                {
                    Player = player,
                    IsReturning = true,
                });
            }
            if (request.Name == null)
            {
                Log.Information("Login request submitted with no name");
                return BadRequest("Must provide a name if the specified player doesn't exist");
            }
            player = await playerService.Create(request.Email, request.Name);
            Log.Debug($"New user created: {player.Email}");
            return Ok(new LoginResponse()
            {
                Player = player,
                IsReturning = false,
            });
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
            Log.Information($"Request for user id {id} returned no results");
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
                Log.Information("GetByEmail user request submitted with no email");
                return BadRequest("`Email` header must be provided");
            }
            var p = await playerService.GetByEmail(email);
            if (p != null)
            {
                return Ok(p);
            }
            Log.Information($"Email {email} was requested, but not found");
            return NotFound();
        }
    }
}