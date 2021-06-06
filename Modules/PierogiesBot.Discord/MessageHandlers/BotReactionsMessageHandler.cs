using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.Discord.Extensions;

namespace PierogiesBot.Discord.MessageHandlers
{
    public class BotReactionsMessageHandler : RuleUpdatingMessageHandlerBase<BotReactRule>, IUserSocketMessageHandler
    {
        public BotReactionsMessageHandler(IRepository<BotReactRule> repository, IMessageBus messageBus,
            ILogger<BotReactionsMessageHandler> logger) : base(messageBus, logger, repository)
        {
        }


        public async Task<IResult> HandleAsync(SocketCommandContext context, int argPos = 0)
        {
            var rule = Rules.Value.FirstOrDefault(r => r.CanExecuteRule(context.Message.Content));
            if (rule is null)
                return ExecuteResult.FromError(CommandError.UnmetPrecondition, "No matching rule for given message");

            var reactions = rule.Reactions.ToList();
            var reaction = reactions.First();
            var reactionEmote = context.Guild.Emotes.FirstOrDefault(e => e.Name.Equals(reaction));

            if (reactionEmote is null)
                return ExecuteResult.FromError(CommandError.Unsuccessful, $"Emote {reaction} not found");

            await context.Message.AddReactionAsync(reactionEmote);
            return ExecuteResult.FromSuccess();
        }
    }
}