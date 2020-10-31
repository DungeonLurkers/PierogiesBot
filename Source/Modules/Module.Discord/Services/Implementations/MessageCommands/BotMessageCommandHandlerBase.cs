using Discord;
using Module.Discord.Services.Definitions;

namespace Module.Discord.Services.Implementations.MessageCommands
{
    public abstract class BotMessageCommandHandlerBase : IMessageCommandHandler
    {
        protected const string CmdPrefix = "=>";
        public void Handle(IMessage message)
        {
            if (!message.Content.StartsWith(CmdPrefix)) return;
            HandleInternal(message);
        }

        public abstract void HandleInternal(IMessage message);


    }
}