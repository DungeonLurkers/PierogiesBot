using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Services;

namespace PierogiesBot.Discord.Modules
{
    [Group("subscribe")]
    public class SubscribeCommandModule : ModuleBase
    {
        private readonly ILogger<SubscribeCommandModule> _logger;
        private readonly ChannelSubscribeService _channelSubscribeService;

        public SubscribeCommandModule(ILogger<SubscribeCommandModule> logger, ChannelSubscribeService channelSubscribeService)
        {
            _logger = logger;
            _channelSubscribeService = channelSubscribeService;
        }
        [Command("responses")]
        public async Task SubscribeResponses()
        {
            _logger.LogInformation("New response subscription on channel {0} guild {1}", Context.Channel.Name, Context.Guild.Name);
            await _channelSubscribeService.Subscribe(Context.Guild, Context.Channel);
        }
        
        [Command("responses")]
        public async Task SubscribeReactions()
        {
            _logger.LogInformation("New response subscription on channel {0} guild {1}", Context.Channel.Name, Context.Guild.Name);
            await _channelSubscribeService.Subscribe(Context.Guild, Context.Channel);
        }
    }
}