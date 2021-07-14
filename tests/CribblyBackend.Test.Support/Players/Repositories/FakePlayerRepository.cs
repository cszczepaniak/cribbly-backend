using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Repositories;

namespace CribblyBackend.Test.Support.Players.Repositories
{
    public class FakePlayerRepository : IPlayerRepository
    {
        private int nextId = 0;
        private readonly Dictionary<int, Player> _idToPlayer = new();
        private readonly Dictionary<string, Player> _authIdToPlayer = new();
        private readonly Dictionary<string, Player> _emailToPlayer = new();
        public Task<Player> CreateAsync(string authProviderId, string email, string name)
        {
            if (_authIdToPlayer.ContainsKey(authProviderId))
            {
                throw new Exception("duplicate auth provider id not allowed");
            }
            if (_emailToPlayer.ContainsKey(email))
            {
                throw new Exception("duplicate email not allowed");
            }

            nextId++;
            var p = new Player
            {
                Id = nextId,
                AuthProviderId = authProviderId,
                Name = name,
                Email = email,
            };
            _idToPlayer[nextId] = p;
            _authIdToPlayer[authProviderId] = p;
            _emailToPlayer[email] = p;
            return Task.FromResult(p);
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
    }
}