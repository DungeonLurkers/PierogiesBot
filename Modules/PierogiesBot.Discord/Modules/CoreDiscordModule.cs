using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace PierogiesBot.Discord.Modules
{
    public class CoreDiscordModule : LoggingModuleBase
    {

        public CoreDiscordModule(ILogger<CoreDiscordModule> logger) : base(logger)
        {
        }

        [Command("ping")]
        [Summary("Ping command")]
        public async Task Ping()
        {
            LogTrace($"Ping");
            await ReplyAsync("Pong!");
        }
    }
}