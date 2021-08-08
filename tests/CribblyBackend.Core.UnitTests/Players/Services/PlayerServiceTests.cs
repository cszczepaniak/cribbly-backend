using System.Threading.Tasks;
using CribblyBackend.Core.Players.Services;
using CribblyBackend.Test.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using CribblyBackend.Test.Support.Players.Repositories;

namespace CribblyBackend.Core.UnitTests.Players.Services
{
    public class PlayerServiceTests
    {
        private readonly IPlayerService _playerService;
        private readonly FakePlayerRepository _fakePlayerRepository;
        public PlayerServiceTests()
        {
            var provider = ServiceProviderFactory.GetProvider();
            _playerService = provider.GetRequiredService<IPlayerService>();
            _fakePlayerRepository = provider.GetRequiredService<FakePlayerRepository>();
        }

        [Fact]
        public async Task GetOrCreateAsyncShouldCreate_IfPlayerDoesNotExist()
        {
            var expPlayer = TestData.Player();
            var actualPlayer = await _playerService.GetOrCreateAsync(expPlayer);

            Assert.Equal(expPlayer.Id, actualPlayer.Id);
            Assert.Equal(expPlayer.AuthProviderId, actualPlayer.AuthProviderId);
            Assert.Equal(expPlayer.Email, actualPlayer.Email);
            Assert.Equal(expPlayer.Name, actualPlayer.Name);
            Assert.False(actualPlayer.IsReturning);
        }

        [Fact]
        public async Task GetOrCreateAsyncShouldGet_IfPlayerDoesExist()
        {

            var p = await _fakePlayerRepository.CreateAsync(TestData.Player());
            var player = await _playerService.GetOrCreateAsync(p);

            Assert.Equal(p.Id, player.Id);
            Assert.Equal(p.AuthProviderId, player.AuthProviderId);
            Assert.Equal(p.Email, player.Email);
            Assert.Equal(p.Name, player.Name);
            Assert.True(player.IsReturning);
        }

        [Fact]
        public async Task GetOrCreateAsyncShouldThrowOnDuplicateEmail_IfCreating()
        {
            var p1 = TestData.Player();
            var p2 = TestData.Player();
            p1.Email = p2.Email;
            await _fakePlayerRepository.CreateAsync(p1);
            await Assert.ThrowsAsync<Exception>(() => _playerService.GetOrCreateAsync(p2));
        }

        [Fact]
        public async Task GetByIdShouldGetCorrectPlayer()
        {
            var testP1 = await _fakePlayerRepository.CreateAsync(TestData.Player());
            var testP2 = await _fakePlayerRepository.CreateAsync(TestData.Player());

            var p1 = await _playerService.GetByIdAsync(testP1.Id);
            Assert.Equal(testP1.Id, p1.Id);
            Assert.Equal(testP1.AuthProviderId, p1.AuthProviderId);
            Assert.Equal(testP1.Email, p1.Email);
            Assert.Equal(testP1.Name, p1.Name);
            var p2 = await _playerService.GetByIdAsync(testP2.Id);
            Assert.Equal(testP2.Id, p2.Id);
            Assert.Equal(testP2.AuthProviderId, p2.AuthProviderId);
            Assert.Equal(testP2.Email, p2.Email);
            Assert.Equal(testP2.Name, p2.Name);
        }

        [Fact]
        public async Task GetByEmailShouldGetCorrectPlayer()
        {
            var testP1 = await _fakePlayerRepository.CreateAsync(TestData.Player());
            var testP2 = await _fakePlayerRepository.CreateAsync(TestData.Player());

            var p1 = await _playerService.GetByEmailAsync(testP1.Email);
            Assert.Equal(testP1.Id, p1.Id);
            Assert.Equal(testP1.AuthProviderId, p1.AuthProviderId);
            Assert.Equal(testP1.Email, p1.Email);
            Assert.Equal(testP1.Name, p1.Name);
            var p2 = await _playerService.GetByEmailAsync(testP2.Email);
            Assert.Equal(testP2.Id, p2.Id);
            Assert.Equal(testP2.AuthProviderId, p2.AuthProviderId);
            Assert.Equal(testP2.Email, p2.Email);
            Assert.Equal(testP2.Name, p2.Name);
        }

        [Fact]
        public async Task GetByAuthProviderIdShouldGetCorrectPlayer()
        {
            var testP1 = await _fakePlayerRepository.CreateAsync(TestData.Player());
            var testP2 = await _fakePlayerRepository.CreateAsync(TestData.Player());

            var p1 = await _playerService.GetByAuthProviderIdAsync(testP1.AuthProviderId);
            Assert.Equal(testP1.Id, p1.Id);
            Assert.Equal(testP1.AuthProviderId, p1.AuthProviderId);
            Assert.Equal(testP1.Email, p1.Email);
            Assert.Equal(testP1.Name, p1.Name);
            var p2 = await _playerService.GetByAuthProviderIdAsync(testP2.AuthProviderId);
            Assert.Equal(testP2.Id, p2.Id);
            Assert.Equal(testP2.AuthProviderId, p2.AuthProviderId);
            Assert.Equal(testP2.Email, p2.Email);
            Assert.Equal(testP2.Name, p2.Name);
        }

        [Fact]
        public async Task GetByAuthProviderIdShouldCacheResult()
        {
            var testPlayer = await _fakePlayerRepository.CreateAsync(TestData.Player());

            var p = await _playerService.GetByAuthProviderIdAsync(testPlayer.AuthProviderId);
            Assert.Equal(testPlayer.AuthProviderId, p.AuthProviderId);
            Assert.Equal(testPlayer.Email, p.Email);
            Assert.Equal(testPlayer.Name, p.Name);

            var cachedPlayer = await _playerService.GetByAuthProviderIdAsync(testPlayer.AuthProviderId);
            Assert.Equal(testPlayer.AuthProviderId, cachedPlayer.AuthProviderId);
            Assert.Equal(testPlayer.Email, cachedPlayer.Email);
            Assert.Equal(testPlayer.Name, cachedPlayer.Name);

            Assert.Equal(1, _fakePlayerRepository.GetNumberOfCalls(nameof(_fakePlayerRepository.GetByAuthProviderIdAsync)));
        }
    }
}