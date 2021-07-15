using System.Collections.Generic;
using System.Security.Claims;

namespace CribblyBackend.Api.Tests.Common.Auth
{
    public class FakeClaimsProvider
    {
        private readonly Claim[] _claims;
        public FakeClaimsProvider(string authId, string email)
        {
            _claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, authId),
                new Claim(ClaimTypes.Email, email)
            };
        }
        public IEnumerable<Claim> GetClaims() => _claims;
    }
}