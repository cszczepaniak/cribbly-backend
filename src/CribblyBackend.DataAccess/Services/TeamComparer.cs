using System.Collections.Generic;
using CribblyBackend.DataAccess.Teams.Models;

namespace CribblyBackend.DataAccess.Services
{
    public class TeamComparer : IEqualityComparer<Team>
    {
        public bool Equals(Team team1, Team team2)
        {
            return team1.Id == team2.Id;
        }

        public int GetHashCode(Team team)
        {
            if (team == null) return 0;
            unchecked
            {
                int hash = 69;
                hash = (hash * 420) + team.Id;
                return hash;
            }
        }
    }
}
