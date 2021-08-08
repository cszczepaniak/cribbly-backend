using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Test.Support.Common;

namespace CribblyBackend.Test.Support.Games.Repositories
{
    public class FakeGameRepository : FakeRepository, IGameRepository
    {
        private readonly Dictionary<int, Game> _gamesById;
        private readonly Dictionary<int, List<Game>> _gamesByTeamId;
        public FakeGameRepository()
        {
            _gamesById = new();
            _gamesByTeamId = new();
        }
        public Task<Game> CreateAsync(Game game)
        {
            game.Id = IncrementId();
            _gamesById[game.Id] = game;
            if (game.Teams == null)
            {
                return Task.FromResult(game);
            }
            foreach (var t in game.Teams)
            {
                if (!_gamesByTeamId.ContainsKey(t.Id))
                {
                    _gamesByTeamId[t.Id] = new();
                }
                _gamesByTeamId[t.Id].Add(game);
            }
            return Task.FromResult(game);
        }

        public Task DeleteAsync(Game game)
        {
            throw new NotImplementedException();
        }

        public Task<Game> GetByIdAsync(int id)
        {
            if (_gamesById.TryGetValue(id, out var game))
            {
                return Task.FromResult(game);
            }
            return Task.FromResult<Game>(null);
        }

        public Task<IEnumerable<Game>> GetByTeamIdAsync(int teamId)
        {
            if (_gamesByTeamId.TryGetValue(teamId, out var games))
            {
                if (games.Count == 0)
                {
                    throw new Exception("No games found!");
                }
                return Task.FromResult<IEnumerable<Game>>(games);
            }
            throw new Exception("No games found!");
        }

        public Task<Game> UpdateAsync(Game game)
        {
            if (!_gamesById.ContainsKey(game.Id))
            {
                return Task.FromResult<Game>(null);
            }
            _gamesById[game.Id] = game;
            return Task.FromResult(game);
        }
    }
}