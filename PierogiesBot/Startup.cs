using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDB;
using AspNetCore.Proxy;
using Autofac;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Orleans.Http;
using PierogiesBot.Commons.RestClient;
using PierogiesBot.Data;
using PierogiesBot.Discord;
using PierogiesBot.Discord.HealthChecks;
using PierogiesBot.Discord.Settings;
using PierogiesBot.Extensions;
using PierogiesBot.Middlewares;
using PierogiesBot.Models;
using PierogiesBot.Settings;

namespace PierogiesBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.
            builder.RegisterModule<AutofacModule>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(sp =>
            {
                var eventAwaiter = new TaskCompletionSource<bool>(false);

                void ClientOnReady()
                {
                    eventAwaiter.SetResult(true);
                }

                var options = sp.GetRequiredService<IOptions<DiscordSettings>>();
                var discordLogger = sp.GetRequiredService<ILogger<DiscordSocketClient>>();
                var settings = options.Value;

                var client = new DiscordSocketClient();

                client.Log += message => Task.Run(() =>
                {

                    var logLevel = message.Severity switch
                    {
                        LogSeverity.Critical => LogLevel.Critical,
                        LogSeverity.Error => LogLevel.Error,
                        LogSeverity.Warning => LogLevel.Warning,
                        LogSeverity.Info => LogLevel.Information,
                        LogSeverity.Verbose => LogLevel.Debug,
                        LogSeverity.Debug => LogLevel.Trace,
                        _ => throw new ArgumentOutOfRangeException(nameof(message), "has wrong LogSeverity!")
                    };
                    if (message.Exception is not null) discordLogger.LogError(message.Exception, message.Message);
                    else discordLogger.Log(logLevel, message.Message);
                });

                client.LoginAsync(TokenType.Bot, settings.Token).ConfigureAwait(false).GetAwaiter().GetResult();
                client.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                client.Ready += () => Task.Run(ClientOnReady);

                eventAwaiter.Task.ConfigureAwait(false).GetAwaiter().GetResult();

                return client!;
            });

            services.AddMongo();
            services.AddDataServices();
            services.AddDiscord();
            services.AddAutoMapper(typeof(Startup), typeof(IPierogiesBotApi));

            services.AddHealthChecks()
                .AddMongoDb(Configuration["MongoDBOption:ConnectionString"], Configuration["MongoDBOption:Database"],
                    name: "MongoDB PierogiesBot collection health check")
                .AddCheck<DiscordHealthCheck>("Discord");

            services.AddHealthChecksUI(settings =>
            {
                settings.SetApiMaxActiveRequests(2);
                settings.SetEvaluationTimeInSeconds(15);
                settings.MaximumHistoryEntriesPerEndpoint(25);
                settings.AddHealthCheckEndpoint("PierogiesBot", "http://localhost:5000/health");
            }).AddInMemoryStorage();

            services.AddCors();
            services.AddControllers();

            services.Configure<DatabaseSettings>(Configuration.GetSection(SettingsSections.DatabaseSettings));
            services.Configure<JwtSettings>(Configuration.GetSection(SettingsSections.JwtSettings));
            services.Configure<DiscordSettings>(Configuration.GetSection(DiscordSettings.SectionName));
            services.Configure<CommandServiceConfig>(Configuration.GetSection(nameof(CommandServiceConfig)));

            services.Configure<MongoDBOption>(Configuration.GetSection(SettingsSections.MongoDbOption))
                .AddMongoDatabase()
                .AddMongoDbContext<AppUser, MongoIdentityRole>()
                .AddMongoStore<AppUser, MongoIdentityRole>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
                options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
                options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Name;
            });

            services.AddIdentity<AppUser, MongoIdentityRole>()
                .AddRoles<MongoIdentityRole>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Secret"])),
                        ValidIssuer = Configuration["JwtSettings:ValidIssuer"],
                        ValidAudience = Configuration["JwtSettings:ValidAudience"],
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role,
                    };
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "PierogiesBot", Version = "v1"});

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        Array.Empty<string>()
                    },
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PierogiesBot v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyMethod());

            app.UseMiddleware<JwtMiddleware>();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    })
                    .RequireHost("localhost");

                endpoints.MapHealthChecksUI().RequireAuthorization();
            });
        }
    }
}