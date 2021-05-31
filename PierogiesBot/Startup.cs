using System;
using System.Security.Claims;
using System.Text;
using AspNetCore.Identity.MongoDB;
using Discord.Commands;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using PierogiesBot.Commons.RestClient;
using PierogiesBot.Data;
using PierogiesBot.Data.Services;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongo();
            services.AddDataServices();
            services.AddDiscord();
            services.AddAutoMapper(typeof(Startup), typeof(IPierogiesBotApi));
            
            services.AddHealthChecks()
                .AddMongoDb(Configuration["MongoDBOption:ConnectionString"], Configuration["MongoDBOption:Database"], name: "MongoDB PierogiesBot collection health check")
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
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Secret"])),
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
                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
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
                                Id = "Bearer"  
                            }  
                        },
                        Array.Empty<string>()

                    }  
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