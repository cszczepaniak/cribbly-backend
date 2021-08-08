using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Games.Repositories;

namespace CribblyBackend.Test.Support.Games.Repositories
{
    public class FakeGameRepository : IGameRepository
    {
        public Task CreateAsync(Game Game)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteAsync(Game Game)
        {
            throw new System.NotImplementedException();
        }

        public Task<Game> GetByIdAsync(int Id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Game>> GetByTeamIdAsync(int teamId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Game> UpdateAsync(Game Game)
        {
            throw new System.NotImplementedException();
        }
    }
}