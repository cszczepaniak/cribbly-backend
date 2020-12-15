using System;
using Microsoft.AspNetCore.Mvc;
using CribblyBackend.Models;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class TournamentController : ControllerBase
    {
        [HttpGet]
        public Tournament Get()
        {
            Tournament tournament = new Tournament();
            tournament.placeholder = "this will soon be a ton of JSON";
            return tournament;
        }
    }
}