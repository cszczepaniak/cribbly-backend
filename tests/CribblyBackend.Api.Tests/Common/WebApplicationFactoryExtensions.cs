using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using CribblyBackend.Api.Tests.Common.Auth;

namespace CribblyBackend.Api.Tests.Common
{
    public static class WebApplicationFactoryExtensions
    {
        private static WebApplicationFactory<T> WithAuthentication<T>(this WebApplicationFactory<T> factory, string authId, string email) where T : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services
                        .AddTransient<FakeClaimsProvider>(_ => new FakeClaimsProvider(authId, email))
                        .AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>("Test", options => { });
                });
            });
        }

        public static HttpClient CreateClientWithTestAuth<T>(this WebApplicationFactory<T> factory, string authId, string email) where T : class
        {
            var client = factory.WithAuthentication(authId, email).CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            return client;
        }
    }
}