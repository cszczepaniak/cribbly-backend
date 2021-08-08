using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Games.Repositories;
using System.Collections.Generic;
namespace CribblyBackend.Core.Games.Services
{
    public interface IGameService
    {
        Task<Game> GetById(int Id);
        Task<Game> Update(Game Game);
        Task<IEnumerable<Game>> GetByTeamAsync(int teamId);
        Task<Game> Create(Game Game);
        Task Delete(Game Game);
    }
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<Game> GetById(int id)
        {
            return await _gameRepository.GetByIdAsync(id);
        }
        public async Task<Game> Create(Game game)
        {
            return await _gameRepository.CreateAsync(game);
        }
        public async Task<Game> Update(Game game)
        {
            return await _gameRepository.UpdateAsync(game);
        }
        public Task Delete(Game game)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Game>> GetByTeamAsync(int teamId)
        {
            return await _gameRepository.GetByTeamIdAsync(teamId);
        }
    }
}