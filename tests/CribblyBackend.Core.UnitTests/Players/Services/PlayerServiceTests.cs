using System.Threading.Tasks;
using CribblyBackend.Core.Players.Services;
using CribblyBackend.Test.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using CribblyBackend.Core.Players.Repositories;
using CribblyBackend.Core.Players.Models;

namespace CribblyBackend.Core.UnitTests.Players.Services
{
    public class PlayerServiceTests
    {
        private readonly IPlayerService _playerService;
        private readonly IPlayerRepository _fakePlayerRepository;
        public PlayerServiceTests()
        {
            var provider = ServiceProviderFactory.GetProvider();
            _playerService = provider.GetRequiredService<IPlayerService>();
            _fakePlayerRepository = provider.GetRequiredService<IPlayerRepository>();
        }

        [Fact]
        public async Task GetOrCreateAsyncShouldCreate_IfPlayerDoesNotExist()
        {
            var player = await _playerService.GetOrCreateAsync(new Player { AuthProviderId = "an auth id", Email = "abc@abc.com", Name = "james bond" });

            Assert.Equal(1, player.Id);
            Assert.Equal("an auth id", player.AuthProviderId);
            Assert.Equal("abc@abc.com", player.Email);
            Assert.Equal("james bond", player.Name);
            Assert.Equal(false, player.IsReturning);
        }

        [Fact]
        public async Task GetOrCreateAsyncShouldGet_IfPlayerDoesExist()
        {
            var player = await _playerService.GetOrCreateAsync(new Player { AuthProviderId = "an auth id", Email = "abc@abc.com", Name = "james bond" });

            Assert.Equal(1, player.Id);
            Assert.Equal("an auth id", player.AuthProviderId);
            Assert.Equal("abc@abc.com", player.Email);
            Assert.Equal("james bond", player.Name);
        }

        [Fact]
        public async Task GetOrCreateAsyncShouldThrowOnDuplicateEmail_IfCreating()
        {
            await _fakePlayerRepository.CreateAsync(new Player { AuthProviderId = "an auth id", Email = "abc@abc.com", Name = "james bond" });
            await Assert.ThrowsAsync<Exception>(
                () => _playerService.GetOrCreateAsync(new Player { AuthProviderId = "an auth id1", Email = "abc@abc.com", Name = "james bond1" })
            );
        }

        [Fact]
        public async Task GetByIdShouldGetCorrectPlayer()
        {
            var testP1 = await _fakePlayerRepository.CreateAsync(
                new Player { AuthProviderId = TestData.NewString(), Email = TestData.NewString(), Name = TestData.NewString() }
            );
            var testP2 = await _fakePlayerRepository.CreateAsync(
                new Player { AuthProviderId = TestData.NewString(), Email = TestData.NewString(), Name = TestData.NewString() }
            );

            var p1 = await _playerService.GetByIdAsync(1);
            Assert.Equal(1, p1.Id);
            Assert.Equal(testP1.AuthProviderId, p1.AuthProviderId);
            Assert.Equal(testP1.Email, p1.Email);
            Assert.Equal(testP1.Name, p1.Name);
            var p2 = await _playerService.GetByIdAsync(2);
            Assert.Equal(2, p2.Id);
            Assert.Equal(testP2.AuthProviderId, p2.AuthProviderId);
            Assert.Equal(testP2.Email, p2.Email);
            Assert.Equal(testP2.Name, p2.Name);
        }

        [Fact]
        public async Task GetByEmailShouldGetCorrectPlayer()
        {
            var email1 = TestData.NewString();
            var email2 = TestData.NewString();
            var testP1 = await _fakePlayerRepository.CreateAsync(
                new Player { AuthProviderId = TestData.NewString(), Email = email1, Name = TestData.NewString() }
            );
            var testP2 = await _fakePlayerRepository.CreateAsync(
                new Player { AuthProviderId = TestData.NewString(), Email = email2, Name = TestData.NewString() }
            );

            var p1 = await _playerService.GetByEmailAsync(email1);
            Assert.Equal(1, p1.Id);
            Assert.Equal(testP1.AuthProviderId, p1.AuthProviderId);
            Assert.Equal(testP1.Email, p1.Email);
            Assert.Equal(testP1.Name, p1.Name);
            var p2 = await _playerService.GetByEmailAsync(email2);
            Assert.Equal(2, p2.Id);
            Assert.Equal(testP2.AuthProviderId, p2.AuthProviderId);
            Assert.Equal(testP2.Email, p2.Email);
            Assert.Equal(testP2.Name, p2.Name);
        }

        [Fact]
        public async Task GetByAuthProviderIdShouldGetCorrectPlayer()
        {
            var authId1 = TestData.NewString();
            var authId2 = TestData.NewString();
            var testP1 = await _fakePlayerRepository.CreateAsync(
                new Player { AuthProviderId = authId1, Email = TestData.NewString(), Name = "1" }
            );
            var testP2 = await _fakePlayerRepository.CreateAsync(
                new Player { AuthProviderId = authId2, Email = TestData.NewString(), Name = "2" }
            );

            var p1 = await _playerService.GetByAuthProviderIdAsync(authId1);
            Assert.Equal(1, p1.Id);
            Assert.Equal(testP1.AuthProviderId, p1.AuthProviderId);
            Assert.Equal(testP1.Email, p1.Email);
            Assert.Equal(testP1.Name, p1.Name);
            var p2 = await _playerService.GetByAuthProviderIdAsync(authId2);
            Assert.Equal(2, p2.Id);
            Assert.Equal(testP2.AuthProviderId, p2.AuthProviderId);
            Assert.Equal(testP2.Email, p2.Email);
            Assert.Equal(testP2.Name, p2.Name);
        }

        [Fact]
        public async Task GetByAuthProviderIdShouldCacheResult()
        {
            var authId = TestData.NewString();
            var testPlayer = await _fakePlayerRepository.CreateAsync(
                new Player { AuthProviderId = authId, Email = TestData.NewString(), Name = TestData.NewString() }
            );

            var p = await _playerService.GetByAuthProviderIdAsync(authId);
            Assert.Equal(authId, p.AuthProviderId);
            Assert.Equal(testPlayer.Email, p.Email);
            Assert.Equal(testPlayer.Name, p.Name);

            _fakePlayerRepository.Update(new Player { Id = testPlayer.Id, AuthProviderId = authId, Name = "super not random", Email = "not random" });
            var updatedPlayer = await _playerService.GetByIdAsync(testPlayer.Id);
            Assert.Equal(authId, updatedPlayer.AuthProviderId);
            Assert.Equal("not random", updatedPlayer.Email);
            Assert.Equal("super not random", updatedPlayer.Name);

            var cachedPlayer = await _playerService.GetByAuthProviderIdAsync(authId);
            Assert.Equal(authId, cachedPlayer.AuthProviderId);
            Assert.Equal(testPlayer.Email, cachedPlayer.Email);
            Assert.Equal(testPlayer.Name, cachedPlayer.Name);
        }
    }
}