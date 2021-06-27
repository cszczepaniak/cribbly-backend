using System.Collections.Generic;

namespace CribblyBackend.DataAccess.Models
{
    public class Division
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
    }
}