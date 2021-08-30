using System.Threading.Tasks;
using Amazon.S3;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Players.Repositories;
using CribblyBackend.DataAccess.Common.S3;

namespace CribblyBackend.DataAccess.Players.Repositories
{
    public class S3PlayerRepository : IPlayerRepository
    {
        private readonly IS3Wrapper _s3;
        public S3PlayerRepository(IS3Wrapper s3)
        {
            _s3 = s3;
        }
        public async Task<Player> CreateAsync(Player player)
        {
            await _s3.PutObjectAsync(GenerateKey(player), player);
            return player;
        }

        public void Delete(Player player)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ExistsAsync(string email)
        {
            throw new System.NotImplementedException();
        }

        public Task<Player> GetByAuthProviderIdAsync(string authProviderId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Player> GetByEmailAsync(string email)
        {
            throw new System.NotImplementedException();
        }

        public Task<Player> GetByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Player player)
        {
            throw new System.NotImplementedException();
        }

        private string GenerateKey(Player player)
        {
            return player.AuthProviderId;
        }
    }
}