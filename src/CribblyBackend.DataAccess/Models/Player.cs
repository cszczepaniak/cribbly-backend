namespace CribblyBackend.DataAccess.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Team Team { get; set; }
        public string Role { get; set; }
    }
}