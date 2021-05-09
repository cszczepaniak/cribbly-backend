using System.Threading.Tasks;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.DataAccess.Players.Repositories;

namespace CribblyBackend.Services
{
    public interface IPlayerService
    {
        Task<bool> Exists(string email);
        Task<Player> GetByEmail(string email);
        Task<Player> GetById(int id);
        void Update(Player player);
        Task<Player> Create(string email, string name);
        void Delete(Player player);
    }
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<bool> Exists(string email)
        {
            return await _playerRepository.Exists(email);
        }

        public async Task<Player> Create(string email, string name)
        {
            return await _playerRepository.Create(email, name);
        }

        public void Delete(Player player)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Player> GetById(int id)
        {
            return await _playerRepository.GetById(id);
        }
        public async Task<Player> GetByEmail(string email)
        {
            return await _playerRepository.GetByEmail(email);
        }

        public void Update(Player player)
        {
            throw new System.NotImplementedException();
        }
    }
}