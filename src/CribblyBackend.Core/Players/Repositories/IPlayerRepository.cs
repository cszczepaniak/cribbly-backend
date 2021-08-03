using System.Threading.Tasks;
using CribblyBackend.Core.Players.Models;

namespace CribblyBackend.Core.Players.Repositories
{
    public interface IPlayerRepository
    {
        Task<bool> ExistsAsync(string email);
        Task<Player> GetByAuthProviderIdAsync(string authProviderId);
        Task<Player> GetByEmailAsync(string email);
        Task<Player> GetByIdAsync(int id);
        void Update(Player player);
        Task<Player> CreateAsync(Player player);
        void Delete(Player player);
    }
}