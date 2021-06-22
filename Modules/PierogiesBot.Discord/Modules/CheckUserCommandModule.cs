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
        public Task WhoIs(SocketUser? user = null)
        {
            LogTrace($"Whois {user}");
            user ??= Context.Client.CurrentUser;

            return ReplyAsync($"{user.Username}#{user.Discriminator}");
        }
    }
}