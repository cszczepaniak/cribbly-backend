using System.Data;
using System.Reflection;
using CribblyBackend.DataAccess.Games.Repositories;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace CribblyBackend.Common
{
    public static class ServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddFirebaseAuthentication(this IServiceCollection services)
        {
            var audience = Config.FirebaseAudience;
            return services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    {
                        options.Authority = $"https://securetoken.google.com/{audience}";
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = $"https://securetoken.google.com/{audience}",
                            ValidateAudience = true,
                            ValidAudience = audience,
                            ValidateLifetime = true
                        };
                    }
                );
        }

        public static IServiceCollection AddCribblyMySql(this IServiceCollection services)
        {
            return services.AddFluentMigratorCore()
                .ConfigureRunner(c => c
                    .AddMySql5()
                    .WithGlobalConnectionString(Config.MySqlConfig.Connection)
                    .ScanIn(Assembly.GetAssembly(typeof(GameQueries))).For.All())
                .AddTransient<IDbConnection>(db => new MySqlConnection(Config.MySqlConfig.Connection));
        }
    }
}