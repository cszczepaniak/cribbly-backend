using System.Data;
using System.Reflection;
using CribblyBackend.Core.Extensions;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.DataAccess;
using CribblyBackend.DataAccess.Games.Repositories;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Serilog;

namespace CribblyBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var audience = Configuration["FirebaseAuth:Audience"];
            services
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
                    });
            services.AddAuthorization();
            services.AddFluentMigratorCore()
                .ConfigureRunner(c => c
                    .AddMySql5()
                    .WithGlobalConnectionString(Configuration["MySQL:ConnectionString"])
                    .ScanIn(Assembly.GetAssembly(typeof(GameQueries))).For.All());

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddSingleton(Log.Logger);
            services.AddTransient<IDbConnection>(db => new MySqlConnection(Configuration["MySQL:ConnectionString"]));

            services.AddCoreServices();
            services.AddDataAccess();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner migrationRunner)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            migrationRunner.MigrateUp();
        }
    }
}
