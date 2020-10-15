using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PierogiesBot.Modules.Core.Enums;
using PierogiesBot.Modules.Core.Extensions;
using PierogiesBot.Modules.Discord.Observables.Implementations;
using PierogiesBot.Modules.Discord.Services.Definitions;

namespace PierogiesBot.Modules.Discord.Services
{
    public class PierogiesBotService : IHostedService
    {
        public const string CommandPrefix = "=>"; 
        private readonly ILogger<PierogiesBotService> _logger;
        private readonly IDiscordBotService _discordBotService;
        private readonly Random _random;

        public PierogiesBotService(ILogger<PierogiesBotService> logger, IDiscordBotService discordBotService)
        {
            _logger = logger;
            _discordBotService = discordBotService;
            _random = new Random((int) DateTime.Now.Ticks);

            InitializeSubscriptions();
        }

        public void InitializeSubscriptions()
        {
            var owos = new List<string>
                {
                    "OwO","Owo","owO","ÓwÓ","ÕwÕ","@w@",
                    "ØwØ","øwø","uwu","☆w☆","✧w✧","♥w♥",
                    "゜w゜","◕w◕","ᅌwᅌ","◔w◔","ʘwʘ","⓪w⓪",
                    "︠ʘw ︠ʘ","(owo)"
                };

            var uwus = new List<string>
            {
                "ᵕ꒳ᵕ", "ᵘ ꒳ ᵘ", "ᵘʷᵘ", "⒰⒲⒰", "🇺🇼🇺", "🆄🆆🆄", "🅄🅆🅄", "પฝપ", "ሁሠሁ", "ⓤⓦⓤ", "🅤🅦🅤", "ｕｗｕ", "ＵｗＵ",
                "𝖴𝗐𝖴", "𝗨𝘄𝗨", "ᵾwᵾ", "𝕌𝕨𝕌", "𝓤𝔀𝓤"
            };
            
            var triggers = new List<string>
            {
                "https://i.ytimg.com/vi/TEN40j3eMx8/maxresdefault.jpg",
                "https://encrypted-tbn0.gstatic.com/images?q=tbn%3AANd9GcT-i7gU8JDB-hXW7uNQpfFUMBFMIoKBFnUx7Q&usqp=CAU",
                "https://www.dictionary.com/e/wp-content/uploads/2018/07/triggered-2.png",
                "https://img-9gag-fun.9cache.com/photo/aGdMp3G_460s.jpg"
            };
            
            _discordBotService.BotStateObservable.Subscribe(state =>
            {
                _logger.LogInformation("Inner bot state changed! New state is {0}", state);
            });

            var messageObservable = _discordBotService.MessageObservable
                .Where(tuple => tuple.changeType == MessageChangeType.Added)
                .Select(tuple => tuple.message);

            var commandObservable = messageObservable.AsBotCommandObservable();


            commandObservable
                .WhereBotCommandIs("sayhi")
                .Do(async commandMsg =>
                {
                    _logger.LogInformation("Sending message...");
                    var typingDisposable = commandMsg.Channel.EnterTypingState();
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    await commandMsg.Channel.SendMessageAsync("Hi!");
                    typingDisposable.Dispose();
                }).Subscribe();
            
            messageObservable
                .WhereMessageContentIs("jebać disa", comparison: StringComparison.InvariantCultureIgnoreCase)
                .Do(async commandMsg =>
                {
                    _logger.LogInformation("Sending message...");
                    var typingDisposable = commandMsg.Channel.EnterTypingState();
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    await commandMsg.Channel.SendMessageAsync("kurwe zwisa");
                    typingDisposable.Dispose();
                }).Subscribe();

            messageObservable
                .WhereMessageContentContains(":v")
                .WhereAuthorUsernameIsNot("PierogiesBot")
                .Throttle(TimeSpan.FromSeconds(1))
                .Do(async message => await message.Channel.SendMessageAsync("v:"))
                .Subscribe();
            
            messageObservable
                .WhereMessageContentContains("v:")
                .WhereAuthorUsernameIsNot("PierogiesBot")
                .Throttle(TimeSpan.FromSeconds(1))
                .Do(async message => await message.Channel.SendMessageAsync(":v"))
                .Subscribe();
            
            commandObservable
                .WhereBotCommandIs("host")
                .LogInfo(tuple => $"Command: =>host. Running hostname command")
                .LogInfo(tuple => $"Entering typing state")
                .Select(message => (message: message, typingDisposable: message.Channel.EnterTypingState()))
                .Select(tuple => (message: tuple.message, typingDisposable: tuple.typingDisposable, commandResult: 
                    NetworkInterface
                        .GetAllNetworkInterfaces()
                        .Select(@interface => @interface.GetIPProperties())
                        .SelectMany(statistics => statistics.UnicastAddresses)
                        .Where(information => information.Address.AddressFamily == AddressFamily.InterNetwork)
                        .Select(information => information.Address)))
                .LogDebug(tuple => $"Hostname result is: {tuple.commandResult}")
                .LogDebug(tuple => $"Publishing results to channel {tuple.message.Channel.Name}")
                .Do(async tuple => await tuple.message.Channel.SendMessageAsync(string.Join(", ", tuple.commandResult.Select(address => address.ToString()))))
                .Do(tuple => tuple.typingDisposable.Dispose())
                .Subscribe();

            messageObservable
                .WhereMessageContentContains("uwu")
                .WhereAuthorUsernameIsNot("PierogiesBot")
                .Do(async message => await SearchForAllMatchesWordAndReport(message, "uwu"))
                .SendRandomFromToCurrentMessageChannelDelayWithTyping(uwus, TimeSpan.FromSeconds(1))
                .Subscribe();
            
            messageObservable
                .WhereMessageContentContains("owo")
                .WhereAuthorUsernameIsNot("PierogiesBot")
                .Do(async message => await SearchForAllMatchesWordAndReport(message, "owo"))
                .SendRandomFromToCurrentMessageChannelDelayWithTyping(owos, TimeSpan.FromSeconds(1))
                .Subscribe();
            
            messageObservable
                .WhereMessageContentContains("horny")
                .WhereAuthorUsernameIsNot("PierogiesBot")
                // .Do(async message => await SearchForAllMatchesWordAndReport(message, "horny"))
                .Do(async message =>
                {
                    var messageMentionedUserIds = message.MentionedUserIds;

                    if (messageMentionedUserIds == null)
                    {
                        await message.Channel.SendMessageAsync($"BONK");
                        return;
                    }
                    var usersMentions = string.Join(", ",
                        messageMentionedUserIds.Select(arg => $@"<@{arg.ToString()}>"));
                    await message.Channel.SendMessageAsync($"BONK {usersMentions}");
                })
                .Subscribe();
            
            messageObservable
                .WhereMessageContentContains("Jestem Kamil Świtek")
                .SendMessageToCurrentMessageChannelDelayWithTyping("Nazywam się Ja i jestem stąd!", TimeSpan.FromSeconds(3))
                .Subscribe();
            
            messageObservable
                .WhereMessageContentNotContains("prawda")
                .WhereMessageContentContains("nie prawda")
                .WhereAuthorUsernameIsNot("PierogiesBot")
                .SendMessageToCurrentMessageChannelDelayWithTyping("Prawda", TimeSpan.FromSeconds(1))
                .Subscribe();
            
            messageObservable
                .WhereMessageContentNotContains("nie prawda")
                .WhereMessageContentNotContains("nieprawda")
                .WhereMessageContentContains("prawda")
                .WhereAuthorUsernameIsNot("PierogiesBot")
                .SendMessageToCurrentMessageChannelDelayWithTyping("Nieprawda", TimeSpan.FromSeconds(1))
                .Subscribe();
            
            messageObservable
                .WhereMessageContentContains("trigger")
                .WhereAuthorUsernameIsNot("PierogiesBot")
                .SendMessageToCurrentMessageChannel("TRIGGERED")
                .SendRandomFromToCurrentMessageChannelDelayWithTyping(triggers, TimeSpan.Zero)
                .Subscribe();

            messageObservable
                .Merge(messageObservable.WhereMessageContentContains("stópkarz"))
                .Merge(messageObservable.WhereMessageContentContains("stopkarz"))
                .WhereMessageContentContains("st00pkarz")
                .WhereAuthorUsernameIsNot("PierogiesBot")
                .SendMessageToCurrentMessageChannel("Fuj, stopy :v")
                .Subscribe();

            CronObservable.Cron(CronObservable.BlazeCronTab, TaskPoolScheduler.Default)
                .Merge(CronObservable.Cron(CronObservable.Blaze2CronTab, TaskPoolScheduler.Default))
                .Select(async i
                    => (channel: await _discordBotService.DiscordClient.GetChannelAsync(655390316148555807),
                        message: "4:20"))
                .Do(async tuple =>
                {
                    var (channel, message) = await tuple;

                    var textChannel = channel as SocketTextChannel;

                    textChannel?.SendMessageAsync(message);
                })
                .Subscribe();

            SendMessageOnCronOccurrence(CronObservable.Jp2CronTab, "2137")
                .Subscribe();


        }

        private IObservable<Task<(IChannel channel, string message)>> SendMessageOnCronOccurrence(string crontab, string message)
        {
            return CronObservable.Cron(crontab, TaskPoolScheduler.Default)
                .Select(async i
                    => (channel: await _discordBotService.DiscordClient.GetChannelAsync(655390316148555807),
                        message: message))
                .Do(async tuple =>
                {
                    var (channel, message) = await tuple;

                    var textChannel = channel as SocketTextChannel;

                    textChannel?.SendMessageAsync(message);
                });
        }

        private static async Task SearchForAllMatchesWordAndReport(IMessage message, string substring)
        {
            var pattern =
                $@"\w*{substring}\w*";
            var matches = Regex.Matches(message.Content, pattern, RegexOptions.IgnoreCase);

            var matchesStringified = string.Join(", ", matches.Select(match => $"'{match.Value}'"));

            await message.Channel.SendMessageAsync($"'{substring}' found in {matchesStringified}!");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Bot");

            var token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");

            if (token is null) return;
            
            await _discordBotService.LoginAsync(token);
            
            await _discordBotService.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Bot");
            await Task.CompletedTask;
        }
    }
}