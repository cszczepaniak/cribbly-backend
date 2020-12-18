using System.Collections.Generic;

namespace CribblyBackend.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Division { get; set; }
        public List<int> Players { get; set; }
        public List<int> GameScores { get; set; }
        public List<int> PlayInGames { get; set; }
        public List<int> BracketGames { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int TotalScore { get; set; }
        public int Ranking { get; set; }
        public int Seed { get; set; }
        public bool InTournament { get; set; }
    }
}