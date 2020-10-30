using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Discord;
using Microsoft.Extensions.Logging;

namespace Module.Discord.Services.Definitions
{
    public class MessageCommandChainImpl : IMessageCommandChain
    {
        private readonly ILogger<MessageCommandChainImpl> _logger;
        private List<IMessageCommandHandler> _handlers;

        public MessageCommandChainImpl(IEnumerable<IMessageCommandHandler> messageCommandHandlers,
            ILogger<MessageCommandChainImpl> logger)
        {
            _logger = logger;
            _handlers = new List<IMessageCommandHandler>(messageCommandHandlers);
        }

        public void HandleMessage(IMessage message)
        {
            foreach (var commandHandler in _handlers)
            {
                try
                {
                    commandHandler.Handle(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception while running message command handler {0}", commandHandler.GetType().Name);
                    throw;
                }
            }
        }

        public IDisposable BindToMessageObservable(IObservable<IMessage> messageObservable)
        {
            return messageObservable
                .Do(HandleMessage)
                .Subscribe();
        }
    }
}