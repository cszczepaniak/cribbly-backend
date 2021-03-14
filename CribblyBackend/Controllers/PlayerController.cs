using System;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Models.Network;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Serilog;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService playerService;
        private readonly ILogger logger;
        public PlayerController(IPlayerService playerService, ILogger logger)
        {
            this.playerService = playerService;
            this.logger = logger;
        }

        /// <summary>
        /// Handles the login. Creates or gets the player specified in the request.
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            logger.Debug("Received login request: {@request}", request);
            if (request.Email == null)
            {
                logger.Information("Login request submitted with no email: {@request}", request);
                return BadRequest("Must provide an email");
            }
            var exists = await playerService.Exists(request.Email);
            Player player;
            if (exists)
            {
                player = await playerService.GetByEmail(request.Email);
                logger.Debug("User {player} successfully logged in", player.Id);
                return Ok(new LoginResponse()
                {
                    Player = player,
                    IsReturning = true,
                });
            }
            if (request.Name == null)
            {
                logger.Information("Login request submitted with no name: {@request}", request);
                return BadRequest("Must provide a name if the specified player doesn't exist");
            }
            player = await playerService.Create(request.Email, request.Name);
            logger.Debug("New user created: {@player}", player);
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
            logger.Debug("Received request to get user using email {id}", id);
            var p = await playerService.GetById(id);
            if (p != null)
            {
                return Ok(p);
            }
            logger.Information("Request for user id {id} returned no results", id);
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
            logger.Debug("Received request to get user using email {email}", email);
            if (!emailHeaderExists)
            {
                logger.Information("GetByEmail user request submitted with no email");
                return BadRequest("`Email` header must be provided");
            }
            var p = await playerService.GetByEmail(email);
            if (p != null)
            {
                return Ok(p);
            }
            logger.Information("User {email} was requested, but not found", email);
            return NotFound();
        }
    }
}