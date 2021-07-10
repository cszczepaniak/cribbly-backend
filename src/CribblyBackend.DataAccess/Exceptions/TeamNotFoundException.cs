using System;
using CribblyBackend.Core.Teams.Models;

namespace CribblyBackend.DataAccess.Exceptions
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