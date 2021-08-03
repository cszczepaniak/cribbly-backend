using CribblyBackend.Core.Teams.Models;

namespace CribblyBackend.Core.Players.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string AuthProviderId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Team Team { get; set; }
        public string Role { get; set; }
        public bool IsReturning { get; set; }
    }
}