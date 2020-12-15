using System;
using System.Collections.Generic;
using CribblyBackend.Models;

namespace CribblyBackend.Interfaces
{
    interface IGame
    {
        List<Team> Teams { get; set; }
        //Specify if game is a PlayIn game or Bracket game
        string Type { get; set; }
        Team Winner { get; set; }
        Team Loser { get; set; }
        int ScoreDifference { get; set; }
    }
}