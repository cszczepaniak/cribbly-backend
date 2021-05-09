using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Core.Players.Repositories;
using CribblyBackend.Core.Teams.Repositories;
using CribblyBackend.Core.Tournaments.Repositories;
using CribblyBackend.DataAccess.Games.Repositories;
using CribblyBackend.DataAccess.Players.Repositories;
using CribblyBackend.DataAccess.Teams.Repositories;
using CribblyBackend.DataAccess.Tournaments.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CribblyBackend.DataAccess
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            return services
                .AddTransient<IPlayerRepository, PlayerRepository>()
                .AddTransient<ITeamRepository, TeamRepository>()
                .AddTransient<IGameRepository, GameRepository>()
                .AddTransient<ITournamentRepository, TournamentRepository>();
        }
    }
}
