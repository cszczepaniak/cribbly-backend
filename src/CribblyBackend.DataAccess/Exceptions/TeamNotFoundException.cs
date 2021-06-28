using System;
using CribblyBackend.DataAccess.Models;

namespace CribblyBackend.DataAccess
{
    public class TeamNotFoundException : Exception
    {
        public TeamNotFoundException()
        {
            
        }

        public TeamNotFoundException(string message)
            : base("Team not found")
        {
            
        }

        public TeamNotFoundException(string message, Exception inner)
            : base("Team not found", inner)
        {
            
        }
    }
}