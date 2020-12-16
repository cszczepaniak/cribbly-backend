using System.Collections.Generic;

namespace CribblyBackend.Models
{
    public class Team
    {
        public string Name { get; set; }
        public string Division { get; set; }
        public List<Player> Players { get; set; }
        public List<int> GameScores { get; set; }
        public List<PlayInGame> PlayInGames { get; set; }
        public List<BracketGame> BracketGames { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int TotalScore { get; set; }
        public int Ranking { get; set; }
        public int Seed { get; set; }
        public bool InTournament { get; set; }
    }
}