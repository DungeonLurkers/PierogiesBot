using System.Threading.Tasks;
using Discord;
using Module.Discord.Services.Definitions;

namespace Module.Discord.Services.Implementations.MessageCommands
{
    public abstract class BotMessageCommandHandlerBase : IMessageCommandHandler
    {
        protected string CmdPrefix { get; private set; }

        protected BotMessageCommandHandlerBase(string cmdPrefix = "=>")
        {
            CmdPrefix = cmdPrefix;
        }
        public async Task Handle(IMessage message)
        {
            if (!message.Content.StartsWith(CmdPrefix)) return;
            await HandleInternal(message);
        }

        public abstract Task HandleInternal(IMessage message);


    }
}