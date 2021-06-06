using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Services;

namespace PierogiesBot.Discord.Modules
{
    [Group("sub")]
    public class SubscribeCommandModule : LoggingModuleBase
    {
        public SubscribeCommandModule(ILogger<SubscribeCommandModule> logger) : base(logger)
        {
        }

        [Group("responses")]
        public class SubscribeResponsesCommandModule : LoggingModuleBase
        {
            private readonly ChannelSubscribeService _channelSubscribeService;
            private readonly ILogger<SubscribeResponsesCommandModule> _logger;

            public SubscribeResponsesCommandModule(ILogger<SubscribeResponsesCommandModule> logger,
                ChannelSubscribeService channelSubscribeService) : base(logger)
            {
                _logger = logger;
                _channelSubscribeService = channelSubscribeService;
            }

            [Command("all")]
            public async Task Subscribe()
            {
                LogTrace("Subscribe to all channels");
                _logger.LogInformation("New response subscription on channel {0} guild {1}", Context.Channel.Name,
                    Context.Guild.Name);

                foreach (var guildChannel in await ((IGuild) Context.Guild).GetChannelsAsync())
                    if (guildChannel is SocketTextChannel channel)
                        await _channelSubscribeService.Subscribe(Context.Guild, channel);

                await ReplyAsync("I will watch ALL channels from now on...");
            }

            [Command("add")]
            public async Task Subscribe(SocketTextChannel channel)
            {
                LogTrace($"Subscribe to channel {channel}");
                _logger.LogInformation("New response subscription on channel {0} guild {1}", Context.Channel.Name,
                    Context.Guild.Name);
                await _channelSubscribeService.Subscribe(Context.Guild, channel);

                await ReplyAsync($"I will watch channel {channel.Name} from now on...");
            }

            [Command("add")]
            public async Task Subscribe(params SocketTextChannel[] channels)
            {
                LogTrace($"Subscribe to channels {string.Join(", ", channels.Select(x => x.Name))}");
                _logger.LogInformation("New response subscription on  multiple channels in guild {0}",
                    Context.Guild.Name);

                foreach (var channel in channels)
                    await _channelSubscribeService.Subscribe(Context.Guild, channel);

                await ReplyAsync(
                    $"I will watch channels {string.Join(", ", channels.Select(c => c.Name))} from now on...");
            }

            [Command("del")]
            public async Task Unsubscribe()
            {
                LogTrace("Unsubscribing all channels");
                _logger.LogInformation("Del response subscription on guild {0}", Context.Guild.Name);

                foreach (var guildChannel in await ((IGuild) Context.Guild).GetChannelsAsync())
                    if (guildChannel is SocketTextChannel channel)
                        await _channelSubscribeService.Unsubscribe(Context.Guild, channel);

                await ReplyAsync("I got bored watching you, bye");
            }

            [Command("del")]
            public async Task Unsubscribe(params SocketTextChannel[] channels)
            {
                LogTrace($"Unsubscribing channels {string.Join(", ", channels.Select(x => x.Name))}");

                foreach (var channel in channels) await _channelSubscribeService.Unsubscribe(Context.Guild, channel);

                await ReplyAsync($"I got bored watching you on {string.Join(", ", channels.Select(x => x.Name))}, bye");
            }
        }

        [Group("crontab")]
        public class SubscribeCrontabCommandModule : LoggingModuleBase
        {
            private readonly CrontabSubscribeService _crontabSubscribeService;
            private readonly ILogger<SubscribeCrontabCommandModule> _logger;

            public SubscribeCrontabCommandModule(ILogger<SubscribeCrontabCommandModule> logger,
                CrontabSubscribeService crontabSubscribeService) : base(logger)
            {
                _logger = logger;
                _crontabSubscribeService = crontabSubscribeService;
            }

            [Command("add")]
            public async Task Subscribe(params SocketTextChannel[] channels)
            {
                LogTrace($"New Crontab subscription to channels {string.Join(",", channels.Select(x => x.Name))}");
                _logger.LogInformation("New crontab subscription on multiple channels in guild {0}",
                    Context.Guild.Name);

                if (!channels.Any())
                {
                    var channel = Context.Channel;
                    await _crontabSubscribeService.Subscribe(Context.Guild, channel);

                    await ReplyAsync($"I will post scheduled messages in {channel}");
                    return;
                }

                foreach (var channel in channels)
                    await _crontabSubscribeService.Subscribe(Context.Guild, channel);

                await ReplyAsync(
                    $"I will post scheduled messages on {string.Join(", ", channels.Select(c => c.ToString()))} from now on");
            }

            [Command("del")]
            public async Task Unsubscribe()
            {
                LogTrace("Unsubscribing from all channels");
                _logger.LogInformation("Del crontab subscription on guild {0}", Context.Guild.Name);

                foreach (var guildChannel in await ((IGuild) Context.Guild).GetChannelsAsync())
                    if (guildChannel is SocketTextChannel channel)
                        await _crontabSubscribeService.Unsubscribe(Context.Guild, channel);

                await ReplyAsync("I got bored posting here, bye");
            }

            [Command("del")]
            public async Task Unsubscribe(SocketTextChannel channel)
            {
                LogTrace($"Unsubscribing from {channel}");
                _logger.LogInformation("Del response subscription on channel {0} guild {1}", Context.Channel.Name,
                    Context.Guild.Name);

                await _crontabSubscribeService.Unsubscribe(Context.Guild, channel);

                await ReplyAsync($"I got bored posting on {channel}, bye");
            }
        }
    }
}