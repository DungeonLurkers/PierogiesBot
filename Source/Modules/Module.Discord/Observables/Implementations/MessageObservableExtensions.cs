using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Discord;
using Module.Discord.Services;

namespace Module.Discord.Observables.Implementations
{
    public static class MessageObservableExtensions
    {
        private static Random _random = new Random();
        public static IObservable<IMessage> AsBotCommandObservable(this IObservable<IMessage> observable)
            => observable
                .Where(message => message.Content.StartsWith(PierogiesBotService.CommandPrefix));

        public static IObservable<IMessage> WhereBotCommandIs(this IObservable<IMessage> observable, string command)
            => observable
                .AsBotCommandObservable()
                .Where(x => x.Content.Substring(2).StartsWith(command, StringComparison.InvariantCultureIgnoreCase));
        
        public static IObservable<IMessage> WhereMessageContentIs(this IObservable<IMessage> observable, string message, 
            StringComparison comparison = StringComparison.InvariantCulture)
            => observable
                .Where(x => x.Content.Equals(message, comparison));

        public static IObservable<IMessage> WhereMessageContentContains(this IObservable<IMessage> observable,
            string substring,
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
            => observable.Where(x => x.Content.Contains(substring, comparison));
        
        public static IObservable<IMessage> WhereMessageContentNotContains(this IObservable<IMessage> observable,
            string substring,
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
            => observable.Where(x => !x.Content.Contains(substring, comparison));
        
        public static IObservable<IMessage> SendMessageToCurrentMessageChannel(this IObservable<IMessage> observable,
            string message)
            => observable.Do(async currentMsg => await currentMsg.Channel.SendMessageAsync(message));
        
        public static IObservable<IMessage> SendMessageToCurrentMessageChannelDelayWithTyping(this IObservable<IMessage> observable,
            string message, TimeSpan typingStateSpan)
            => observable.Do(async currentMsg =>
            {
                var typingDisposable = currentMsg.Channel.EnterTypingState();
                await Task.Delay(typingStateSpan);
                typingDisposable.Dispose();
                await currentMsg.Channel.SendMessageAsync(message);
            });
        
        public static IObservable<IMessage> SendRandomFromToCurrentMessageChannelDelayWithTyping(this IObservable<IMessage> observable,
            IList<string> from, TimeSpan typingStateSpan)
            => observable.Do(async currentMsg =>
            {
                var msg = from.ElementAt(_random.Next(from.Count - 1))!;
                var typingDisposable = currentMsg.Channel.EnterTypingState();
                await Task.Delay(typingStateSpan);
                typingDisposable.Dispose();
                await currentMsg.Channel.SendMessageAsync(msg);
            });

        public static IObservable<IMessage> WhereChannelNameIs(this IObservable<IMessage> observable, string channelName)
            => observable.Where(message => message.Channel.Name.Equals(channelName));
        
        public static IObservable<IMessage> WhereChannelNameIsNot(this IObservable<IMessage> observable, string channelName)
            => observable.Where(message => !message.Channel.Name.Equals(channelName));
        
        public static IObservable<IMessage> WhereAuthorUsernameIs(this IObservable<IMessage> observable, string username)
            => observable.Where(message => message.Author.Username.Equals(username));
        
        public static IObservable<IMessage> WhereAuthorUsernameIsNot(this IObservable<IMessage> observable, string username)
            => observable.Where(message => !message.Author.Username.Equals(username));
    }
}