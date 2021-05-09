using System.Collections.Generic;
using CribblyBackend.DataAccess.Divisions.Models;
using CribblyBackend.DataAccess.Games.Models;
using CribblyBackend.DataAccess.Players.Models;

namespace CribblyBackend.DataAccess.Teams.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Division Division { get; set; }
        public List<Player> Players { get; set; }
        public List<int> GameScores { get; set; }
        public List<Game> PlayInGames { get; set; }
        public List<Game> BracketGames { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int TotalScore { get; set; }
        public int Ranking { get; set; }
        public int Seed { get; set; }
        public bool InTournament { get; set; }
    }
}