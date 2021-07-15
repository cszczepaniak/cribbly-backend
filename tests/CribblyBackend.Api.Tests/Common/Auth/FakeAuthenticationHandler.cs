using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CribblyBackend.Test.Support;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CribblyBackend.Api.Tests.Common.Auth
{
    public class FakeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly FakeClaimsProvider _provider;
        public FakeAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, FakeClaimsProvider provider) : base(options, logger, encoder, clock)
        {
            _provider = provider;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(_provider.GetClaims(), "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}