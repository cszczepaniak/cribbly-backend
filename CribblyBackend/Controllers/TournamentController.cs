using Microsoft.AspNetCore.Mvc;
using CribblyBackend.Services;
using System.Threading.Tasks;

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

        [HttpGet]
        public async Task<IActionResult> GetNextTournament()
        {
            var nextTournament = await tournamentService.GetNextTournament();
            if (nextTournament == null)
            {
                return NotFound();
            }
            return Ok(nextTournament);
        }
    }
}