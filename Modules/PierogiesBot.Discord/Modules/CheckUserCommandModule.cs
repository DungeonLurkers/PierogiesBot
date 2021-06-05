using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace PierogiesBot.Discord.Modules
{
    public class CheckUserCommandModule : LoggingModuleBase
    {

        public CheckUserCommandModule(ILogger<CheckUserCommandModule> logger) : base(logger)
        {
        }
        [Command("whois")]
        public async Task WhoIs(SocketUser? user = null)
        {
            LogTrace($"Whois {user}");
            user ??= Context.Client.CurrentUser;

            await ReplyAsync($"{user.Username}#{user.Discriminator}");
        }
    }
}