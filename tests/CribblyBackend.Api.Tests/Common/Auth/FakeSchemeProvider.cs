using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace CribblyBackend.Api.Tests.Common.Auth
{
    public class FakeSchemeProvider : AuthenticationSchemeProvider
    {
        public FakeSchemeProvider(IOptions<AuthenticationOptions> options) : base(options) { }

        public override Task<AuthenticationScheme> GetSchemeAsync(string name)
        {
            if (name == "Test")
            {
                var scheme = new AuthenticationScheme("Test", "Test", typeof(FakeAuthenticationHandler));
                return Task.FromResult(scheme);
            }
            return base.GetSchemeAsync(name);
        }
    }
}