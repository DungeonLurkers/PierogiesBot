using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace Module.Discord.Services.Definitions
{
    public interface IMessageCommandChain
    {
        Task HandleMessage(IMessage message);

        IDisposable BindToMessageObservable(IObservable<IMessage> messageObservable);
    }
}