using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using CribblyBackend.Api.Tests.Common.Auth;
using CribblyBackend.Core.Players.Models;
using CribblyBackend.Test.Support;

namespace CribblyBackend.Api.Tests.Common
{
    public static class WebApplicationFactoryExtensions
    {

        public static HttpClient CreateAuthenticatedClient<T>(this WebApplicationFactory<T> factory, string authId, string email) where T : class
        {
            var fakeClaimsProvider = factory.Services.GetRequiredService<FakeClaimsProvider>();
            fakeClaimsProvider.AddUser(authId, email);
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            return client;
        }

        public static HttpClient CreateAuthenticatedClient<T>(this WebApplicationFactory<T> factory, Player p) where T : class
        {
            return factory.CreateAuthenticatedClient(p.AuthProviderId, p.Email);
        }

        public static HttpClient CreateAuthenticatedClient<T>(this WebApplicationFactory<T> factory) where T : class
        {
            return factory.CreateAuthenticatedClient(TestData.NewString(), TestData.NewString());
        }
    }
}