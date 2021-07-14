using System.Threading.Tasks;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace CribblyBackend.Core.Players.Services
{
    public interface IPlayerService
    {
        Task<bool> ExistsAsync(string email);
        Task<Player> GetByAuthProviderIdAsync(string authProviderId);
        Task<Player> GetByEmailAsync(string email);
        Task<Player> GetByIdAsync(int id);
        void Update(Player player);
        Task<Player> CreateAsync(string authProviderId, string email, string name);
        void Delete(Player player);
    }
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMemoryCache _cache;

        public PlayerService(IPlayerRepository playerRepository, IMemoryCache cache)
        {
            _playerRepository = playerRepository;
            _cache = cache;
        }

        public async Task<bool> ExistsAsync(string authProviderId)
        {
            return await _playerRepository.ExistsAsync(authProviderId);
        }

        public async Task<Player> CreateAsync(string authProviderId, string email, string name)
        {
            return await _playerRepository.CreateAsync(authProviderId, email, name);
        }

        public void Delete(Player player)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Player> GetByIdAsync(int id)
        {
            return await _playerRepository.GetByIdAsync(id);
        }
        public async Task<Player> GetByEmailAsync(string email)
        {
            return await _playerRepository.GetByEmailAsync(email);
        }
        public async Task<Player> GetByAuthProviderIdAsync(string authProviderId)
        {
            return await _cache.GetOrCreateAsync(authProviderId, async entry =>
            {
                return await _playerRepository.GetByAuthProviderIdAsync(authProviderId);
            });
        }

        public void Update(Player player)
        {
            throw new System.NotImplementedException();
        }
    }
}