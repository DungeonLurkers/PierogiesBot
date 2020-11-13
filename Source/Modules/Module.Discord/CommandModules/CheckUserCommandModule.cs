using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Module.Discord.CommandModules
{
    public class CheckUserCommandModule : ModuleBase<SocketCommandContext>
    {
        [Command("whois")]
        public async Task WhoIs(SocketUser? user)
        {
            user ??= Context.Client.CurrentUser;

            await ReplyAsync($"{user.Username}#{user.Discriminator}");
        }
    }
}