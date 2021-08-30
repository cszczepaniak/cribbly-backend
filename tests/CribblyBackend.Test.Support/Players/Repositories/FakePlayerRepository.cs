using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Repositories;

namespace CribblyBackend.Test.Support.Players.Repositories
{
    public class FakePlayerRepository : IPlayerRepository
    {
        private string someId = Guid.NewGuid().ToString();
        private int nextId = 0;
        private readonly object _nextIdLock;
        private readonly Dictionary<int, Player> _idToPlayer;
        private readonly Dictionary<string, Player> _authIdToPlayer;
        private readonly Dictionary<string, Player> _emailToPlayer;
        private readonly Dictionary<string, int> _methodCalls;
        public FakePlayerRepository()
        {
            _idToPlayer = new();
            _authIdToPlayer = new();
            _emailToPlayer = new();
            _methodCalls = new();
            _nextIdLock = new();
        }
        public Task<Player> CreateAsync(Player player)
        {
            if (_authIdToPlayer.ContainsKey(player.AuthProviderId))
            {
                throw new Exception("duplicate auth provider id not allowed");
            }
            if (_emailToPlayer.ContainsKey(player.Email))
            {
                throw new Exception("duplicate email not allowed");
            }

            lock (_nextIdLock)
            {
                nextId++;
            }
            var p = new Player
            {
                Id = nextId,
                AuthProviderId = player.AuthProviderId,
                Name = player.Name,
                Email = player.Email,
            };
            _idToPlayer[nextId] = p;
            _authIdToPlayer[player.AuthProviderId] = p;
            _emailToPlayer[player.Email] = p;
            return Task.FromResult(p);
        }

        public Task DeleteAsync(Player player)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ExistsAsync(string authProviderId)
        {
            return Task.FromResult(_authIdToPlayer.ContainsKey(authProviderId));
        }

        public Task<Player> GetByAuthProviderIdAsync(string authProviderId)
        {
            IncrementMethodCall(nameof(GetByAuthProviderIdAsync));
            if (_authIdToPlayer.TryGetValue(authProviderId, out var p))
            {
                return Task.FromResult(p);
            }
            return Task.FromResult<Player>(null);
        }

        public Task<Player> GetByEmailAsync(string email)
        {
            if (_emailToPlayer.TryGetValue(email, out var p))
            {
                return Task.FromResult(p);
            }
            return Task.FromResult<Player>(null);
        }

        public Task<Player> GetByIdAsync(int id)
        {
            if (_idToPlayer.TryGetValue(id, out var p))
            {
                return Task.FromResult(p);
            }
            return Task.FromResult<Player>(null);
        }

        public Task UpdateAsync(Player player)
        {
            if (!_idToPlayer.ContainsKey(player.Id))
            {
                throw new Exception("player not found");
            }
            _idToPlayer[player.Id] = player;
            return Task.CompletedTask;
        }

        private void IncrementMethodCall(string methodName)
        {
            if (!_methodCalls.ContainsKey(methodName))
            {
                _methodCalls[methodName] = 1;
                return;
            }
            _methodCalls[methodName]++;
        }

        public int GetNumberOfCalls(string methodName)
        {
            return _methodCalls[methodName];
        }
    }
}