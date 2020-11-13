using System.Threading.Tasks;
using Discord.Commands;

namespace Module.Discord.CommandModules
{
    public class CoreDiscordModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping() => await ReplyAsync("Pong!");
    }
}