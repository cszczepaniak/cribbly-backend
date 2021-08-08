using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Core.Teams.Models;

namespace CribblyBackend.Test.Support
{
    public static class TestData
    {
        public static string NewString() => $"{Guid.NewGuid()}";

        public static Player NewPlayer(string authProviderId = null, string email = null, string name = null)
        {
            return new Player
            {
                AuthProviderId = authProviderId ?? TestData.NewString(),
                Email = email ?? TestData.NewString(),
                Name = name ?? TestData.NewString(),
            };
        }

        public static ClaimsPrincipal NewIdentityUser(string authProviderId, string email = "")
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, authProviderId),
            };
            if (!string.IsNullOrEmpty(email))
            {
                claims.Add(new(ClaimTypes.Email, email));
            }
            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        public static IEnumerable<Team> CreateTeams(int n)
        {
            return Enumerable.Range(0, n).Select(_ => new Team
            {
                Name = $"{TestData.NewString()}",
                Players = Enumerable.Range(0, 2).Select(i => new Player
                {
                    Id = i + 1,
                    Email = $"{TestData.NewString()}@test.com",
                    Name = $"{TestData.NewString()}"
                }).ToList(),
            });
        }
    }
}