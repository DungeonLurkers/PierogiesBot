using System;
using System.Threading.Tasks;
using Discord;
using PierogiesBot.Host.Models;

namespace PierogiesBot.Host.Services.Definitions
{
    public interface IDiscordBotService
    {
        IObservable<BotState> BotStateObservable { get;}
        IObservable<(MessageChangeType changeType, IMessage message)> MessageObservable { get;}
        Task LoginAsync(string token);

        Task StartAsync();
        Task StopAsync();
    }
}