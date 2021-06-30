using CribblyBackend.Core.Divisions;
using CribblyBackend.Core.Divisions.Services;
using CribblyBackend.Core.Games.Services;
using CribblyBackend.Core.Players.Services;
using CribblyBackend.Core.Teams.Services;
using CribblyBackend.Core.Tournaments.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CribblyBackend.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IPlayerService, PlayerService>()
                .AddTransient<ITeamService, TeamService>()
                .AddTransient<IGameService, GameService>()
                .AddTransient<ITournamentService, TournamentService>()
                .AddTransient<IDivisionService, DivisionService>()
                .AddTransient<IStandingsService, StandingsService>();
        }
    }
}
