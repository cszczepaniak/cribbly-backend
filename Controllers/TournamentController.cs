using System;
using Microsoft.AspNetCore.Mvc;
using CribblyBackend.Models;
using System.Collections.Generic;
using CribblyBackend.Interfaces;

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
            tournament.Games = new List<IGame>();
                PlayInGame testgame1 = new PlayInGame();
                BracketGame testgame2 = new BracketGame();
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