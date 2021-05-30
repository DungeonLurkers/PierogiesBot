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
        [Command("add")]
        public async Task Subscribe()
        {
            _logger.LogInformation("New response subscription on channel {0} guild {1}", Context.Channel.Name, Context.Guild.Name);
            await _channelSubscribeService.Subscribe(Context.Guild, Context.Channel);

            await ReplyAsync("I will watch this channel from now on...");
        }
        
        [Command("del")]
        public async Task Unsubscribe()
        {
            _logger.LogInformation("New response subscription on channel {0} guild {1}", Context.Channel.Name, Context.Guild.Name);
            await _channelSubscribeService.Subscribe(Context.Guild, Context.Channel);

            await ReplyAsync("I will watch this channel from now on...");
        }
    }
}