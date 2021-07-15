using System;
using System.Net.Http;
using CribblyBackend.Api.Tests.Common.Auth;
using CribblyBackend.Test.Support.Extensions;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace CribblyBackend.Api.Tests.Common
{
    public abstract class ApiTestBase
    {
        protected readonly WebApplicationFactory<Startup> _factory;

        public ApiTestBase()
        {
            _factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services
                        .ReplaceRepositoriesWithFakes()
                        .RemoveAll<IMigrationRunner>()
                        .AddSingleton<FakeClaimsProvider>(new FakeClaimsProvider())
                        .AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>("Test", options => { });
                });
            });
        }
    }
}