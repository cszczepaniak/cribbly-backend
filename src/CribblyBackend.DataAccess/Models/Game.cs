using System.Collections.Generic;

namespace CribblyBackend.DataAccess.Models
{
    public class Game
    {
        public enum Round
        {
            Round1 = 1,
            Round2 = 2,
            Round3 = 3,
            TourneyRound1 = 4,
            Sweet16 = 5,
            QuarterFinal = 6,
            SemiFinal = 7,
            Final = 8
        }
        public int Id { get; set; }
        public List<Team> Teams { get; set; }
        public Team Winner { get; set; }
        public int ScoreDifference { get; set; }
        public Round GameRound { get; set; }

    }
}