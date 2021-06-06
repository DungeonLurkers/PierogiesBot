using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.MessageHandlers;

namespace PierogiesBot.Discord.Services
{
    public class MessageHandlerChain : IMessageHandlerChain
    {
        private readonly ILogger<MessageHandlerChain> _logger;
        private readonly IEnumerable<IUserSocketMessageHandler> _messageHandlers;

        public MessageHandlerChain(ILogger<MessageHandlerChain> logger,
            IEnumerable<IUserSocketMessageHandler> messageHandlers)
        {
            _logger = logger;
            _messageHandlers = messageHandlers;
        }

        public async Task<IResult> HandleAsync(SocketCommandContext context, int argPos = 0)
        {
            try
            {
                foreach (var handler in _messageHandlers)
                {
                    var result = await handler.HandleAsync(context, argPos);
                    if (result.IsSuccess)
                        return ExecuteResult.FromSuccess();
                }

                return ExecuteResult.FromError(CommandError.UnmetPrecondition,
                    "There is no currently registered handler for that message");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while handling new message!");
                return ExecuteResult.FromError(e);
            }
        }
    }
}