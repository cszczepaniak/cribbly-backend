using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;

namespace CribblyBackend.Services
{
    public interface IGameService
    {
        Task<Game> GetById(int Id);
        Task<Game> Update(Game Game);
        Task Create(Game Game);
        void Delete(Game Game);
    }
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger _logger;

        public GameService(IGameRepository gameRepository, ILogger logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<Game> GetById(int id)
        {
            return await _gameRepository.GetById(id);
        }
        public async Task Create(Game game)
        {
            await _gameRepository.Create(game);
        }
        public async Task<Game> Update(Game game)
        {
            try
            {
                return await _gameRepository.Update(game);
            }
            catch (System.Exception e)
            {
                _logger.Error(e, "Database error updating game");
                throw;
            }

        }
        public void Delete(Game game)
        {
            throw new System.NotImplementedException();
        }
    }
}