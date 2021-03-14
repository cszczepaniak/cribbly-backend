using System.Collections.Generic;

namespace CribblyBackend.DataAccess.Models
{
    internal class Division
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
    }
}