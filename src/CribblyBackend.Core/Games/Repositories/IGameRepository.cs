using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;

namespace CribblyBackend.Core.Games.Repositories
{
    public interface IGameRepository
    {
        Task<Game> GetById(int Id);
        void Update(Game Game);
        Task Create(Game Game);
        void Delete(Game Game);
    }
}