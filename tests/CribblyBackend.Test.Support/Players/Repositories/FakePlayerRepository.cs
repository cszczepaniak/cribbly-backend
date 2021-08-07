using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Repositories;

namespace CribblyBackend.Test.Support.Players.Repositories
{
    public class FakePlayerRepository : IPlayerRepository
    {
        private int nextId;
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
            nextId = 0;
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
            Interlocked.Increment(ref nextId);
            player.Id = nextId;
            _idToPlayer[player.Id] = player;
            _authIdToPlayer[player.AuthProviderId] = player;
            _emailToPlayer[player.Email] = player;
            return Task.FromResult(player);
        }

        public void Delete(Player player)
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

        public void Update(Player player)
        {
            if (!_idToPlayer.ContainsKey(player.Id))
            {
                throw new Exception("player not found");
            }
            _idToPlayer[player.Id] = player;
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