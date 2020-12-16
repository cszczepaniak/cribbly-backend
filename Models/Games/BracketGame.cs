using System;
using System.Collections.Generic;
using CribblyBackend.Interfaces;

namespace CribblyBackend.Models
{
    public class BracketGame : IGame
    {
        public List<Team> Teams { get; set; }
        public Team Winner { get; set; }
        public int ScoreDifference { get; set; }
        public string GameRound { get; set; }
    }
}