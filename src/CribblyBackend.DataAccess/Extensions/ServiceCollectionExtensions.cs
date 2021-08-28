using System;
using CribblyBackend.Core.Divisions.Repositories;
using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Core.Players.Repositories;
using CribblyBackend.Core.Teams.Repositories;
using CribblyBackend.Core.Tournaments.Repositories;
using CribblyBackend.DataAccess.Divisions;
using CribblyBackend.DataAccess.Games.Repositories;
using CribblyBackend.DataAccess.Players.Repositories;
using CribblyBackend.DataAccess.Teams.Repositories;
using CribblyBackend.DataAccess.Tournaments.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CribblyBackend.DataAccess
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            var persister = Environment.GetEnvironmentVariable("CRIBBLY_PERSISTER").ToLower();
            Log.Information("Using {@persister} for persistence", persister);
            return services.AddSelectedPersister(persister);
        }
        private static IServiceCollection AddSelectedPersister(this IServiceCollection services, string persister)
        {
            switch (persister)
            {
                case Persisters.MySQL:
                    return services
                        .AddTransient<IPlayerRepository, PlayerRepository>()
                        .AddTransient<ITeamRepository, TeamRepository>()
                        .AddTransient<IGameRepository, GameRepository>()
                        .AddTransient<IDivisionRepository, DivisionRepository>()
                        .AddTransient<ITournamentRepository, TournamentRepository>()
                        .AddTransient<ITournamentPlayerRepository, TournamentPlayerRepository>();
                case Persisters.S3:
                    throw new NotImplementedException("S3 persistence is not implemented");
                case Persisters.Memory:
                    throw new NotImplementedException("In-memory persistence is not implemented");
                case "":
                    var opts = string.Join(", ", Persisters.S3, Persisters.MySQL, Persisters.Memory);
                    throw new Exception($"No persister specified. Please set environment variable CRIBBLY_PERSISTER to one of: {opts}");
                default:
                    throw new ArgumentException($"Unexpected persistence option: {persister}");
            }
        }
    }
}
