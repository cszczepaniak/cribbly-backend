using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CribblyBackend.Test.Support
{
    public static class TestData
    {
        public static string NewString()
        {
            return $"{Guid.NewGuid()}";
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