using System;
using System.Collections.Generic;
using CribblyBackend.Interfaces;

namespace CribblyBackend.Models
{
    public class PlayInGame : IGame
    {
        public int Id { get; set; }
        public List<Team> Teams { get; set; }
        //Specify if game is a PlayIn game or Bracket game
        public Team Winner { get; set; }
        public int GameRound { get; set; }
        public int ScoreDifference { get; set; }
    }
}