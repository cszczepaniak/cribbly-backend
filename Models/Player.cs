using System;

namespace CribblyBackend.Models
{
    public class Player
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool HasTeam { get; set; }
        public string Role { get; set; }
    }
}