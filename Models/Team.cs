using System;
using System.Collections.Generic;

namespace CribblyBackend.Models
{
    public class Team
    {
        public string Name { get; set; }
        public List<Player> Players { get; set; }
    }
}