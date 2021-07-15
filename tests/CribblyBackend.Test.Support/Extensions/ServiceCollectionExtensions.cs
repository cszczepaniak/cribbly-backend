using CribblyBackend.Core.Players.Repositories;
using CribblyBackend.Test.Support.Players.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CribblyBackend.Test.Support.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ReplaceRepositoriesWithFakes(this IServiceCollection services)
        {
            return services.ReplaceService<IPlayerRepository, FakePlayerRepository>();
        }

        private static IServiceCollection ReplaceService<TInterface, TNewImpl>(this IServiceCollection services)
            where TInterface : class
            where TNewImpl : class, TInterface, new()
        {
            return services
                .RemoveAll<TInterface>()
                .AddSingleton<TNewImpl>()
                .AddSingleton<TInterface>(p => p.GetRequiredService<TNewImpl>());
        }
    }
}