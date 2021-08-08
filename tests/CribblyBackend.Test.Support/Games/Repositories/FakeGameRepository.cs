using System;
using System.Collections.Generic;
using System.Threading;
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
        public Task CreateAsync(Game game)
        {
            Interlocked.Increment(ref nextId);
            game.Id = IncrementId();
            _gamesById[game.Id] = game;
            foreach (var t in game.Teams)
            {
                if (!_gamesByTeamId.ContainsKey(t.Id))
                {
                    _gamesByTeamId[t.Id] = new();
                }
                _gamesByTeamId[t.Id].Add(game);
            }
            return Task.CompletedTask;
        }

        public void DeleteAsync(Game game)
        {
            throw new NotImplementedException();
        }

        public Task<Game> GetByIdAsync(int id)
        {
            if (_gamesById.TryGetValue(id, out var game))
            {
                return Task.FromResult(game);
            }
            throw new Exception("Game not found");
        }

        public Task<IEnumerable<Game>> GetByTeamIdAsync(int teamId)
        {
            if (_gamesByTeamId.TryGetValue(teamId, out var games))
            {
                if (games.Count == 0)
                {
                    throw new Exception("No games found!");
                }
                return Task.FromResult((IEnumerable<Game>)games);
            }
            throw new Exception("No games found!");
        }

        public Task<Game> UpdateAsync(Game game)
        {
            if (!_gamesById.ContainsKey(game.Id))
            {
                throw new Exception("Game not found!");
            }
            _gamesById[game.Id] = game;
            return Task.FromResult(game);
        }
    }
}