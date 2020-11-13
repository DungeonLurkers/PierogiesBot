using System.Threading.Tasks;
using Discord;

namespace Module.Discord.Services.Definitions
{
    public interface IMessageCommandHandler
    {
        Task Handle(IMessage message);
    }
}