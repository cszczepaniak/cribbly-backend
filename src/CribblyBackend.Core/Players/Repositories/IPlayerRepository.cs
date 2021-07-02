using System.Threading.Tasks;
using CribblyBackend.Core.Players.Models;

namespace CribblyBackend.Core.Players.Repositories
{
    public interface IPlayerRepository
    {
        Task<bool> Exists(string email);
        Task<Player> GetByEmail(string email);
        Task<Player> GetById(int id);
        void Update(Player player);
        Task<Player> Create(string email, string name);
        void Delete(Player player);
    }
}