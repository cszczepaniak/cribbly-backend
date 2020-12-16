using System;
using System.Collections.Generic;
using CribblyBackend.Models;

namespace CribblyBackend.Interfaces
{
    public interface IGame
    {
        List<Team> Teams { get; set; }
        //Specify if game is a PlayIn game or Bracket game
        Team Winner { get; set; }
        int ScoreDifference { get; set; }
    }
}