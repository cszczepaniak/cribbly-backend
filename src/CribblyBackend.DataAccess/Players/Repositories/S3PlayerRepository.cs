using System;
using System.Text;
using System.Threading.Tasks;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Repositories;
using CribblyBackend.DataAccess.Common.S3;
using CribblyBackend.DataAccess.Players.Models;

namespace CribblyBackend.DataAccess.Players.Repositories
{
    public class S3PlayerRepository : IPlayerRepository
    {
        private readonly IS3Wrapper _s3;
        private const string BaseKey = "players";
        public S3PlayerRepository(IS3Wrapper s3)
        {
            _s3 = s3;
        }
        public async Task<Player> CreateAsync(Player player)
        {
            // we have to store players under auth id _and_ email so we can look them up by either
            await Task.WhenAll(
                _s3.PutObjectAsync(GenerateAuthProviderIdKey(player.AuthProviderId), player),
                _s3.PutObjectAsync(GenerateEmailKey(player.Email), player)
            );
            return player;
        }

        public Task DeleteAsync(Player player)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> ExistsAsync(string authProviderId)
        {
            var (_, exists) = await _s3.GetObjectAsync<S3Player>(GenerateEmailKey(authProviderId));
            return exists;
        }

        public async Task<Player> GetByAuthProviderIdAsync(string authProviderId)
        {
            var (p, _) = await _s3.GetObjectAsync<S3Player>(GenerateAuthProviderIdKey(authProviderId));
            return p.ToPlayer();
        }

        public async Task<Player> GetByEmailAsync(string email)
        {
            var (p, _) = await _s3.GetObjectAsync<S3Player>(GenerateEmailKey(email));
            return p.ToPlayer();
        }

        public Task<Player> GetByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateAsync(Player player)
        {
            await _s3.PutObjectAsync(GenerateAuthProviderIdKey(player.AuthProviderId), player);
        }
        private string GenerateAuthProviderIdKey(string authProviderId)
        {
            return $"{BaseKey}/authid/{authProviderId}";
        }

        private string GenerateEmailKey(string email)
        {
            // generate a string from the hex representation of each character so we don't have to worry about
            // @ or . being in the key
            var bs = Encoding.Default.GetBytes(email);
            return $"{BaseKey}/email/{BitConverter.ToString(bs).Replace("-", "")}";
        }
    }
}