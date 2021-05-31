using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PierogiesBot.Discord.Modules
{
    [Group("react")]
    public class ReactionCommandModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("Reacts to last message before command with a specified reaction")]
        public async Task React([Summary("Reaction name")] string reactionName)
        {
            var messagesBefore = await Context.Channel
                .GetMessagesAsync(Context.Message, Direction.Before, 1)
                .FlattenAsync();

            var messageBefore = messagesBefore?.FirstOrDefault();
            if (messageBefore is null) return;

            var emote = Context
                .Guild.Emotes
                .FirstOrDefault(e => e.Name.Equals(reactionName, StringComparison.InvariantCultureIgnoreCase));
            
            if(emote is null) return;
            
            await messageBefore.AddReactionAsync(emote);
        }
        
        [Command]
        [Summary("Reacts to a given message with a specified reaction")]
        public async Task React([Summary("Message ID")] ulong messageSnowflakeId, [Summary("Reaction name")] string reactionName)
        {
            var message = await Context.Channel.GetMessageAsync(messageSnowflakeId);

            var emote = Context
                .Guild.Emotes
                .FirstOrDefault(e => e.Name.Equals(reactionName, StringComparison.InvariantCultureIgnoreCase));
            
            if(emote is null) return;
            
            await message.AddReactionAsync(emote);
        }
    }
}