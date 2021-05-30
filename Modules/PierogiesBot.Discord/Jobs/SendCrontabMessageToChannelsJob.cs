using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using Quartz;

namespace PierogiesBot.Discord.Jobs
{
    public class SendCrontabMessageToChannelsJob : IJob
    {
        private readonly ILogger<SendCrontabMessageToChannelsJob> _logger;
        private readonly DiscordSocketClient _client;
        private readonly IRepository<BotMessageSubscription> _subscriptions;

        public SendCrontabMessageToChannelsJob(ILogger<SendCrontabMessageToChannelsJob> logger, DiscordSocketClient client, IRepository<BotMessageSubscription> subscriptions)
        {
            _logger = logger;
            _client = client;
            _subscriptions = subscriptions;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            if (context.MergedJobDataMap["Rule"] is BotCrontabRule rule)
            {
                _logger.LogDebug("Running job trigger fron crontab rule {0}", rule.Crontab);
                var subs = await _subscriptions.GetAll();
                
                foreach (var sub in subs)
                {
                    await HandleSubscription(sub, rule);
                }
                
            }
        }

        private async Task HandleSubscription(BotMessageSubscription? sub, BotCrontabRule rule)
        {
            var channel = (SocketTextChannel) _client.GetChannel(sub.ChannelId);
            var guild = channel.Guild;
            if (rule.IsEmoji)
            {
                var foundEmotes = guild.Emotes.FirstOrDefault(x => x.Name.Equals(rule.ReplyEmoji.First()));

                if (foundEmotes is not null)
                    await channel.SendMessageAsync(foundEmotes.ToString());
            }
            else
            {
                await channel.SendMessageAsync(rule.ReplyMessages.First());
            }
        }
    }
}