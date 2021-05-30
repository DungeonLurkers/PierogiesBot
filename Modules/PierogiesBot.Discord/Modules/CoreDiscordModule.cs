using System.Threading.Tasks;
using Discord.Commands;

namespace PierogiesBot.Discord.Modules
{
    public class CoreDiscordModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Ping command")]
        public async Task Ping() => await ReplyAsync("Pong!");
    }
}