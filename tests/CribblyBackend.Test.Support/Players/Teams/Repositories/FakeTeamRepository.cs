using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Teams.Repositories;

namespace CribblyBackend.Test.Support.Players.Teams.Repositories
{
    public class FakeTeamRepository : ITeamRepository
    {
        public Task<int> Create(Team Team)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Team>> Get()
        {
            throw new System.NotImplementedException();
        }

        public Task<Team> GetById(int Id)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Team Team)
        {
            throw new System.NotImplementedException();
        }
    }
}