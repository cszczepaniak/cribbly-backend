using System;
using System.Collections.Generic;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Teams.Models;

namespace CribblyBackend.Core.Tournaments.Models
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