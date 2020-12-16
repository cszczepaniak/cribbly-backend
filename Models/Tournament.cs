using System;
using System.Collections.Generic;
using CribblyBackend.Interfaces;

namespace CribblyBackend.Models
{
    public class Tournament
    {
        public int Year { get; set; }
        public List<Player> Players { get; set; }
        public List<Team> Teams { get; set; }
        public List<IGame> Games { get; set; }
        public Team Champion { get; set; }
    }
}