using System.Collections.Generic;

namespace CribblyBackend.Models
{
    public class Tournament
    {
        public int Year { get; set; }
        public List<Player> Players { get; set; }
        public List<Team> Teams { get; set; }
        public List<PlayInGame> PlayInGames { get; set; }
        public List<BracketGame> BracketGames { get; set; }
        public Team Champion { get; set; }
    }
}