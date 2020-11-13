using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace Module.Discord.Services.Implementations.MessageCommands
{
    public class BotReactCommandHandler : BotMessageCommandHandlerBase
    {
        public BotReactCommandHandler() : base(cmdPrefix: "=>react ")
        {
        }
        public override async Task HandleInternal(IMessage message)
        {
            var content = message.Content;

            var reactName = content?.Substring(0, CmdPrefix.Length - 1);
            
            if (string.IsNullOrWhiteSpace(reactName)) return;
            
            
            
            var channel = message.Channel;
            var messagesToCheck = channel.GetMessagesAsync(message, Direction.Before, 1);

            var flattenedMessages = messagesToCheck.Flatten();

            var messageToReact = await flattenedMessages.SingleAsync();
            
        }
    }
}