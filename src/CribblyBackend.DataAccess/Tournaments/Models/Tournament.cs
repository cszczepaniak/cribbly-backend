using System;
using System.Collections.Generic;
using CribblyBackend.DataAccess.Games.Models;
using CribblyBackend.DataAccess.Players.Models;
using CribblyBackend.DataAccess.Teams.Models;


namespace CribblyBackend.DataAccess.Tournaments.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsOpenForRegistration { get; set; }
        public bool IsActive { get; set; }
        public List<Player> Players { get; set; }
        public List<Team> Teams { get; set; }
        public List<Game> Games { get; set; }
        public Team Champion { get; set; }
    }
}