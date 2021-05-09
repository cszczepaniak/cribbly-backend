using System.Collections.Generic;
using CribblyBackend.DataAccess.Teams.Models;

namespace CribblyBackend.DataAccess.Divisions.Models
{
    public class Division
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
    }
}