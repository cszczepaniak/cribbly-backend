using System;
using CribblyBackend.Core.Extensions;
using CribblyBackend.DataAccess;
using CribblyBackend.Test.Support.Extensions;
using CribblyBackend.Test.Support.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CribblyBackend.Test.Support
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider GetProvider()
        {
            var services = new ServiceCollection();
            services
                .AddDataAccess()
                .ReplaceRepositoriesWithFakes()
                .AddCoreServices()
                .AddSingleton<ILogger, FakeLogger>()
                .AddControllers();
            var provider = services.BuildServiceProvider();
            return provider;
        }
    }
}
