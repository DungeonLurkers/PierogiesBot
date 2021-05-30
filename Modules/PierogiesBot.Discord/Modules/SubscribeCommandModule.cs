using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Services;

namespace PierogiesBot.Discord.Modules
{
    [Group("sub")]
    public class SubscribeCommandModule : ModuleBase
    {
        private readonly ILogger<SubscribeCommandModule> _logger;
        private readonly ChannelSubscribeService _channelSubscribeService;

        public SubscribeCommandModule(ILogger<SubscribeCommandModule> logger, ChannelSubscribeService channelSubscribeService)
        {
            _logger = logger;
            _channelSubscribeService = channelSubscribeService;
        }
        [Command("all")]
        public async Task Subscribe()
        {
            _logger.LogInformation("New response subscription on channel {0} guild {1}", Context.Channel.Name, Context.Guild.Name);
            
            foreach (var guildChannel in await Context.Guild.GetChannelsAsync())
            {
                if (guildChannel is SocketTextChannel channel)
                    await _channelSubscribeService.Subscribe(Context.Guild, channel);
            }

            await ReplyAsync("I will watch ALL channels from now on...");
        }
        
        [Command("add")]
        public async Task Subscribe(SocketTextChannel channel)
        {
            _logger.LogInformation("New response subscription on channel {0} guild {1}", Context.Channel.Name, Context.Guild.Name);
            await _channelSubscribeService.Subscribe(Context.Guild, channel);

            await ReplyAsync($"I will watch channel {channel.Name} from now on...");
        }
        
        [Command("add")]
        public async Task Subscribe(params SocketTextChannel[] channels)
        {
            _logger.LogInformation("New response subscription on  multiple channels in guild {0}", Context.Guild.Name);
            
            foreach (var channel in channels)
                await _channelSubscribeService.Subscribe(Context.Guild, channel);

            await ReplyAsync($"I will watch channels {string.Join(", ", channels.Select(c => c.Name))} from now on...");
        }
        
        [Command("del")]
        public async Task Unsubscribe()
        {
            _logger.LogInformation("Del response subscription on guild {0}", Context.Guild.Name);
            
            foreach (var guildChannel in await Context.Guild.GetChannelsAsync())
            {
                if (guildChannel is SocketTextChannel channel)
                    await _channelSubscribeService.Unsubscribe(Context.Guild, channel);
            }

            await ReplyAsync("I got bored watching you, bye");
        }
        
        [Command("del")]
        public async Task Unsubscribe(SocketTextChannel channel)
        {
            _logger.LogInformation("Del response subscription on channel {0} guild {1}", Context.Channel.Name, Context.Guild.Name);
            
            await _channelSubscribeService.Unsubscribe(Context.Guild, channel);

            await ReplyAsync($"I got bored watching you on {channel.Name}, bye");
        }
    }
}