using System;
using System.Collections.Generic;
using System.Security.Claims;
using CribblyBackend.Core.Players.Models;

namespace CribblyBackend.Test.Support
{
    public static class TestData
    {
        public static string NewString()
        {
            return $"{Guid.NewGuid()}";
        }

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
    }
}