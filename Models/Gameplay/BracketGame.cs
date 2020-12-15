using System;
using System.Collections.Generic;
using CribblyBackend.Interfaces;

namespace CribblyBackend.Models
{
    public class BracketGame : IGame
    {
        public List<Team> Teams { get; set; }
        //Specify if game is a PlayIn game or Bracket game
        public string Type { get; set; }
        public Team Winner { get; set; }
        public int ScoreDifference { get; set; }
        public int GameRound { get; set; }
    }
}