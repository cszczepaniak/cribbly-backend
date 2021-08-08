using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;

namespace CribblyBackend.Core.Games.Repositories
{
    public interface IGameRepository
    {
        Task<Game> GetByIdAsync(int id);
        Task<Game> UpdateAsync(Game game);
        Task<Game> CreateAsync(Game game);
        Task DeleteAsync(Game game);
        Task<IEnumerable<Game>> GetByTeamIdAsync(int teamId);
    }
}