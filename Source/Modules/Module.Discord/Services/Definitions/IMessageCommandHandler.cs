using Discord;

namespace Module.Discord.Services.Definitions
{
    public interface IMessageCommandHandler
    {
        void Handle(IMessage message);
    }
}