using System.Threading.Tasks;
using Discord.Commands;

namespace PierogiesBot.Discord.Services
{
    public interface IMessageHandlerChain
    {
        Task<IResult> HandleAsync(SocketCommandContext context, int argPos = 0);
    }
}