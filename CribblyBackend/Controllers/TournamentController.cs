using Microsoft.AspNetCore.Mvc;
using CribblyBackend.Services;
using System.Threading.Tasks;
using CribblyBackend.Models;
using CribblyBackend.Models.Network;
using Microsoft.AspNetCore.Http;
using System;

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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Bad time: ${e.Message}");
            }
        }
    }
}