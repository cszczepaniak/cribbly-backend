using System;
using System.Data;
using System.Reflection;
using CribblyBackend.Core.Divisions.Repositories;
using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Core.Players.Repositories;
using CribblyBackend.Core.Teams.Repositories;
using CribblyBackend.Core.Tournaments.Repositories;
using CribblyBackend.DataAccess.Common.Config;
using CribblyBackend.DataAccess.Common.S3;
using CribblyBackend.DataAccess.Divisions;
using CribblyBackend.DataAccess.Games.Repositories;
using CribblyBackend.DataAccess.Players.Repositories;
using CribblyBackend.DataAccess.Teams.Repositories;
using CribblyBackend.DataAccess.Tournaments.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Serilog;

namespace CribblyBackend.DataAccess
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            var persister = Persister.Get();
            Log.Information("Using {@persister} for persistence", persister);
            return services.AddSelectedPersister(persister);
        }
        private static IServiceCollection AddSelectedPersister(this IServiceCollection services, string persister)
        {
            services.AddS3Support().AddMysqlSupport();
            switch (persister)
            {
                case Persisters.MySQL:
                    return services.AddMysqlPersistence();
                case Persisters.S3:
                    return services.AddS3Persistence();
                case Persisters.Memory:
                    return services.AddMemoryPersistence();
                case "":
                    var opts = string.Join(", ", Persisters.S3, Persisters.MySQL, Persisters.Memory);
                    throw new Exception($"No persister specified. Please set environment variable CRIBBLY_PERSISTER to one of: {opts}");
                default:
                    throw new ArgumentException($"Unexpected persistence option: {persister}");
            }
        }

        private static IServiceCollection AddS3Support(this IServiceCollection services)
        {
            var bucket = Environment.GetEnvironmentVariable("CRIBBLY_BUCKET");
            if (string.IsNullOrEmpty(bucket))
            {
                throw new Exception("Must set environment variable CRIBBLY_BUCKET if using S3 persister");
            }
            return services.AddSingleton<IS3Wrapper>(_ => new S3Wrapper(bucket));
        }

        private static IServiceCollection AddMemoryPersistence(this IServiceCollection services)
        {
            throw new NotImplementedException("In-memory persistence is not implemented");
        }

        private static IServiceCollection AddS3Persistence(this IServiceCollection services)
        {
            throw new NotImplementedException("S3 persistence is not implemented");
        }

        private static IServiceCollection AddMysqlSupport(this IServiceCollection services)
        {
            return services
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c
                    .AddMySql5()
                    .WithGlobalConnectionString(Config.MySqlConfig.Connection)
                    .ScanIn(Assembly.GetAssembly(typeof(GameQueries))).For.All())
                .AddTransient<IDbConnection>(db => new MySqlConnection(Config.MySqlConfig.Connection));
        }

        private static IServiceCollection AddMysqlPersistence(this IServiceCollection services)
        {
            return services
                .AddTransient<IPlayerRepository, PlayerRepository>()
                .AddTransient<ITeamRepository, TeamRepository>()
                .AddTransient<IGameRepository, GameRepository>()
                .AddTransient<IDivisionRepository, DivisionRepository>()
                .AddTransient<ITournamentRepository, TournamentRepository>()
                .AddTransient<ITournamentPlayerRepository, TournamentPlayerRepository>();
        }
    }
}
