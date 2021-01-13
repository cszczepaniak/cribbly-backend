using System.Data;
using System.Reflection;
using CribblyBackend.Services;
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
                    .ScanIn(Assembly.GetExecutingAssembly()).For.All());

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddTransient<IDbConnection>(db => new MySqlConnection(Configuration["MySQL:ConnectionString"]));
            services.AddTransient<IPlayerService, PlayerService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<ITournamentService, TournamentService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner migrationRunner)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
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
