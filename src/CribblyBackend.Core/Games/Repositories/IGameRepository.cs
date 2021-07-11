using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;

namespace CribblyBackend.Core.Games.Repositories
{
    public interface IGameRepository
    {
        Task<Game> GetById(int Id);
        Task<Game> Update(Game Game);
        Task Create(Game Game);
        void Delete(Game Game);
        Task<IEnumerable<Game>> GetByTeamId(int teamId);
    }
}