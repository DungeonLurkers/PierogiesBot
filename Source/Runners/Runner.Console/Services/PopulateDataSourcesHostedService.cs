using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Module.Core.Enums;
using Module.Data.Models;
using Module.Data.Storage;
using Module.Discord.Services.Definitions;

namespace Runner.Console.Services
{
    public class PopulateDataSourcesHostedService : IHostedService
    {
        private readonly IDataSource<RoleEntity, ulong> _roleDataSource;
        private readonly IDataSource<GuildEntity, ulong> _guildDataSource;
        private readonly IDataSource<GuildUserEntity, ulong> _guildUserDataSource;
        private readonly IDataSource<SettingEntity, Guid> _settingDataSource;
        private readonly IDataSource<BotResponseRule, Guid> _rulesDataSource;
        private readonly IDataSource<BotReactRule, Guid> _reactRulesDataSource;
        private readonly IDiscordBotService _discordBotService;
        private readonly IDiscordClient _discordClient;
        private readonly ILogger<PopulateDataSourcesHostedService> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public PopulateDataSourcesHostedService(
            IDataSource<RoleEntity, ulong> roleDataSource, IDataSource<GuildEntity, ulong> guildDataSource,
            IDataSource<GuildUserEntity, ulong> guildUserDataSource,
            IDataSource<SettingEntity, Guid> settingDataSource,
            IDataSource<BotResponseRule, Guid> rulesDataSource,
            IDataSource<BotReactRule, Guid> reactRulesDataSource,
            IDiscordBotService discordBotService,
            ILogger<PopulateDataSourcesHostedService> logger, IHostApplicationLifetime applicationLifetime)
        {
            _roleDataSource = roleDataSource;
            _guildDataSource = guildDataSource;
            _guildUserDataSource = guildUserDataSource;
            _settingDataSource = settingDataSource;
            _rulesDataSource = rulesDataSource;
            _reactRulesDataSource = reactRulesDataSource;
            _discordBotService = discordBotService;
            _discordClient = discordBotService.DiscordClient;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(PopulateDataSources);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private void PopulateDataSources()
        {
            var resetEvent = new ManualResetEventSlim();
            resetEvent.Reset();
            _discordBotService.BotStateObservable
                .Where(state => state == BotState.Ready)
                .Subscribe(s =>
                {
                    _logger.LogDebug("Discord client is ready!");
                    resetEvent.Set();
                });

            resetEvent.Wait();
            Task.Run(async () =>
            {
                _logger.LogDebug("Fetching guilds...");
                var guilds = await _discordClient.GetGuildsAsync();

                if (guilds != null && guilds.Any())
                {
                    _logger.LogDebug("Fetched {0} guilds", guilds.Count);
                    var guildEntities = guilds.Select(x => new GuildEntity(x)).ToList();

                    _logger.LogDebug("Saving guilds to database");
                    _guildDataSource.AddOrUpdateRange(guildEntities);
                    _logger.LogDebug("Saved guilds to database");

                    var guild = guilds.First(g => g.Id == 182523210175086594);

                    var users = (await guild.GetUsersAsync())?.Select(user => new GuildUserEntity(user));
                    var roles = guild.Roles?.Select(role => new RoleEntity(role));

                    _logger.LogDebug("Constructing responding rules");

                    if (users != null && roles != null)
                    {
                        PopulateDataSource(roles, _roleDataSource);
                        PopulateDataSource(users, _guildUserDataSource);
                    }

                    _logger.LogInformation("All data sources populated");
                }
            });
        }

        private void PopulateDataSource<T, TId>(IEnumerable<T> entities, IDataSource<T, TId> dataSource) where T : class
        {
            var enumerable = entities.ToList();
            if (!enumerable.Any()) return;

            _logger.LogInformation("Saving [{0}] to database", typeof(T).Name);
            dataSource.AddOrUpdateRange(enumerable);
            _logger.LogInformation("Saved!");
        }
    }
}