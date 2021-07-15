using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using CribblyBackend.Test.Support.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;

namespace CribblyBackend.Api.Tests.Common
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services
                    .ReplaceRepositoriesWithFakes()
                    .RemoveAll<IMigrationRunner>();
            });
        }
    }
}