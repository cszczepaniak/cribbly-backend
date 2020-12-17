using Microsoft.AspNetCore.Mvc;
using CribblyBackend.Models;
using System.Collections.Generic;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class TournamentController : ControllerBase
    {
        [HttpGet]
        public Tournament Get()
        {
            //Generate mock data and return to client
            Tournament tournament = new Tournament();
            tournament.Games = new List<Game>();
                Game testgame1 = new Game();
                Game testgame2 = new Game();
                tournament.Games.Add(testgame1);
                tournament.Games.Add(testgame2);
            tournament.Teams = new List<Team>();
                tournament.Teams.Add(new Team());
            tournament.Players = new List<Player>();
                tournament.Players.Add(new Player());
            tournament.Year = 2021;

            return tournament;
        }
    }
}