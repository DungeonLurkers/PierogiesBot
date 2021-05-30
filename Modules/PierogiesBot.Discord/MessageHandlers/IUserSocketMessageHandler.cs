using System.Threading.Tasks;
using Discord.Commands;

namespace PierogiesBot.Discord.MessageHandlers
{
    public interface IUserSocketMessageHandler
    {
        Task<IResult> HandleAsync(SocketCommandContext context, int argPos = 0);
    }
}