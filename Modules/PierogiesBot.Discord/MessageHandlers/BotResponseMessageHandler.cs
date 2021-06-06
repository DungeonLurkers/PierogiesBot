using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Enums;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.Discord.Extensions;

namespace PierogiesBot.Discord.MessageHandlers
{
    public class BotResponseMessageHandler : RuleUpdatingMessageHandlerBase<BotResponseRule>, IUserSocketMessageHandler
    {
        private readonly Random _random;

        public BotResponseMessageHandler(IRepository<BotResponseRule> repository, IMessageBus messageBus,
            ILogger<BotResponseMessageHandler> logger) : base(messageBus, logger, repository)
        {
            _random = new Random();
        }


        public async Task<IResult> HandleAsync(SocketCommandContext context, int argPos = 0)
        {
            var rule = Rules.Value.FirstOrDefault(r => r.CanExecuteRule(context.Message.Content));
            if (rule is null)
                return ExecuteResult.FromError(CommandError.UnmetPrecondition, "No matching rule for given message");

            var ruleResponses = rule.Responses.ToList();
            switch (rule.ResponseMode)
            {
                case ResponseMode.Unknown:
                    break;
                case ResponseMode.First:
                    await context.Channel.SendMessageAsync(ruleResponses.First());
                    break;
                case ResponseMode.Random:
                    await context.Channel.SendMessageAsync(ruleResponses[_random.Next(ruleResponses.Count - 1)]);
                    break;
                default:
                    return ExecuteResult.FromError(new ArgumentOutOfRangeException(nameof(BotResponseRule.ResponseMode),
                        "not known ResponseMode"));
            }

            return ExecuteResult.FromSuccess();
        }
    }
}