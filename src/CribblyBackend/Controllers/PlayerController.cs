using System.Threading.Tasks;
using CribblyBackend.Common;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Services;
using CribblyBackend.Network;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var email = User.GetEmail();
            var authProviderId = User.GetAuthProviderId();
            logger.Debug("Received login request: {@request}", request);
            if (string.IsNullOrWhiteSpace(email))
            {
                logger.Information("No email found in user token", request);
                return BadRequest("Must provide an email");
            }
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                logger.Information("Login request submitted with no name: {@request}", request);
                return BadRequest("Must provide a name if the specified player doesn't exist");
            }
            var player = await playerService.GetOrCreateAsync(
                new Player { AuthProviderId = authProviderId, Email = email, Name = request.Name }
            );
            return Ok(player);
        }

        [HttpPost("test")]
        public async Task<IActionResult> CreateForTest([FromQuery] string name, [FromQuery] string email, [FromQuery] string authProviderId)
        {
            var player = await playerService.GetOrCreateAsync(
                new() { AuthProviderId = authProviderId, Email = email, Name = name }
            );
            return Ok(player);
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
            var p = await playerService.GetByIdAsync(id);
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
            var p = await playerService.GetByEmailAsync(email);
            if (p != null)
            {
                return Ok(p);
            }
            logger.Information("User {email} was requested, but not found", email);
            return NotFound();
        }
    }
}