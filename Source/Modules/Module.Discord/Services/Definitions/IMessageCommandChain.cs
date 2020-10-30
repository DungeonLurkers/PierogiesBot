using System;
using System.Collections.Generic;
using Discord;

namespace Module.Discord.Services.Definitions
{
    public interface IMessageCommandChain
    {
        void HandleMessage(IMessage message);

        IDisposable BindToMessageObservable(IObservable<IMessage> messageObservable);
    }
}