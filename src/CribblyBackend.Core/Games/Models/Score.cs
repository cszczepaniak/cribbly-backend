namespace CribblyBackend.Core.Games.Models
{
    public class Score
    {
        public int GameId { get; set; }
        public int TeamId { get; set; }
        public int GameScore { get; set; }
    }
}