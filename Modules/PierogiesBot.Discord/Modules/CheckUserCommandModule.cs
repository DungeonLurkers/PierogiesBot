using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace PierogiesBot.Discord.Modules
{
    public class CheckUserCommandModule : ModuleBase<SocketCommandContext>
    {
        [Command("whois")]
        public async Task WhoIs(SocketUser? user = null)
        {
            user ??= Context.Client.CurrentUser;

            await ReplyAsync($"{user.Username}#{user.Discriminator}");
        }
    }
}