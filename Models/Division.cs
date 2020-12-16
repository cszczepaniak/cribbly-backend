using System;
using System.Collections.Generic;

namespace CribblyBackend.Models
{
    public class Division
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
    }
}