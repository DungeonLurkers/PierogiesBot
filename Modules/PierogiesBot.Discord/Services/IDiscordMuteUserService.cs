using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using PierogiesBot.Commons.Dtos.Mute;

namespace PierogiesBot.Discord.Services
{
    public interface IDiscordMuteUserService
    {
        Task<IEnumerable<GetMuteDto>> GetAllMutes();

        Task<GetMuteDto?> GetMuteForUser(SocketGuildUser user);

        Task MuteUser(SocketGuildUser user, CreateMuteDto dto);

        Task UnmuteUser(SocketGuildUser user);
    }
}