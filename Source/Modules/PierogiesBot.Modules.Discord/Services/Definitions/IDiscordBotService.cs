using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using PierogiesBot.Modules.Core.Enums;

namespace PierogiesBot.Modules.Discord.Services.Definitions
{
    public interface IDiscordBotService
    {
        IDiscordClient DiscordClient { get; }
        IObservable<BotState> BotStateObservable { get;}
        IObservable<(MessageChangeType changeType, IMessage message)> MessageObservable { get;}
        Task LoginAsync(string token);

        Task StartAsync();
        Task StopAsync();

        Task<IEnumerable<IUser>> GetUsersAsync();
        Task<IEnumerable<IMessageChannel>> GetMessageChannelsAsync();
        Task<IDMChannel?> GetMessageChannelAsync(ulong id);
    }
}