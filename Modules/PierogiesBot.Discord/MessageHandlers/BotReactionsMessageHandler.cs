using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PierogiesBot.Commons.Enums;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.Discord.Extensions;

namespace PierogiesBot.Discord.MessageHandlers
{
    public class BotReactionsMessageHandler : IUserSocketMessageHandler
    {
        private readonly IRepository<BotReactRule> _repository;
        private Lazy<List<BotReactRule>> _rules;

        public BotReactionsMessageHandler(IRepository<BotReactRule> repository)
        {
            _repository = repository;

            _rules = new Lazy<List<BotReactRule>>(() => _repository.GetAll().GetAwaiter().GetResult().ToList());
        }

        public async Task<IResult> HandleAsync(SocketCommandContext context, int argPos = 0)
        {
            var rule = _rules.Value.FirstOrDefault(r => r.CanExecuteRule(context.Message.Content));
            if (rule is null) return ExecuteResult.FromError(CommandError.UnmetPrecondition, "No matching rule for given message");

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