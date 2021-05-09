using System;
using System.Threading.Tasks;
using CribblyBackend.Core.Tournaments.Services;
using CribblyBackend.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService tournamentService;
        public TournamentController(ITournamentService tournamentService)
        {
            this.tournamentService = tournamentService;
        }

        [HttpGet("next")]
        public async Task<IActionResult> GetNextTournament()
        {
            var nextTournament = await tournamentService.GetNextTournament();
            if (nextTournament == null)
            {
                return NotFound();
            }
            return Ok(nextTournament);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentRequest request)
        {
            try
            {
                await tournamentService.Create(request.Date);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Bad time: {e.Message}");
            }
        }

        [HttpPost("setFlags")]
        public async Task<IActionResult> ChangeTournamentFlags([FromBody] ChangeTournamentFlagsRequest request)
        {
            if (!request.IsActive.HasValue && !request.IsOpenForRegistration.HasValue)
            {
                return BadRequest("Must set at least one flag");
            }
            try
            {

                if (request.IsActive.HasValue)
                {
                    await tournamentService.ChangeActiveStatus(request.Id, request.IsActive.Value);
                }
                if (request.IsOpenForRegistration.HasValue)
                {
                    await tournamentService.ChangeOpenForRegistrationStatus(request.Id, request.IsOpenForRegistration.Value);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Bad time: {e.Message}");
            }
        }
    }
}