using System.Collections.Generic;

namespace CribblyBackend.Models
{
    public class Game
    {
        public List<Team> Teams { get; set; }
        public Team Winner { get; set; }
        public int ScoreDifference { get; set; }
    }
}