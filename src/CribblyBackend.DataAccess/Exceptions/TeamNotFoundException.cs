using System;
using CribblyBackend.Core.Teams.Models;

namespace CribblyBackend.DataAccess.Exceptions
{
    public class TeamNotFoundException : Exception
    {
        public TeamNotFoundException(int id)
            :base($"Team with id {id} not found")
        {
            
        }
    }
}