using System.Collections.Generic;
using CribblyBackend.Core.Teams.Models;

namespace CribblyBackend.Core.Divisions.Models
{
    public class Division
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
    }
}