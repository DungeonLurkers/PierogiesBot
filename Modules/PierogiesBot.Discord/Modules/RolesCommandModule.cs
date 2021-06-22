using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace PierogiesBot.Discord.Modules
{
    [RequireUserPermission(GuildPermission.ManageRoles)]
    [Group("role")]
    public class RolesCommandModule : LoggingModuleBase
    {
        public RolesCommandModule(ILogger<RolesCommandModule> logger)
            : base(logger)
        {
        }

        [Command("add")]
        public async Task AddRoleToUser(SocketRole role, SocketGuildUser user)
        {
            LogTrace($"Add role {role} to user {user}");
            await user.AddRoleAsync(role);
            await ReplyAsync($"{user} now has role {role}");
        }

        [Command("remove")]
        public async Task RemoveRoleToUser(SocketRole role, SocketGuildUser user)
        {
            LogTrace($"Add role {role} to user {user}");
            await user.RemoveRoleAsync(role);
            await ReplyAsync($"{user} lost role {role}");
        }
    }
}