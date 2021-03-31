using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Module.Discord.CommandModules
{
    public class EmojiCommandModule : ModuleBase<SocketCommandContext>
    {
        [Command("react")]
        public async Task React(string reactionName)
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
        
        [Command("react")]
        public async Task React(ulong messageSnowflakeId, string reactionName)
        {
            var message = await Context.Channel.GetMessageAsync(messageSnowflakeId);

            var emote = Context
                .Guild.Emotes
                .FirstOrDefault(e => e.Name.Equals(reactionName, StringComparison.InvariantCultureIgnoreCase));
            
            if(emote is null) return;
            
            await message.AddReactionAsync(emote);
        }

        [Command("emoji")]
        public async Task Emoji(params string[] emojis)
        {
            var emotes = Context.Guild.Emotes;
            IEnumerable<GuildEmote?> msgEmotes = emojis.Select(s =>
                emotes.FirstOrDefault(emote => emote.Name.Equals(s, StringComparison.InvariantCultureIgnoreCase)));

            var guildEmotes = msgEmotes.ToList();
            var notFoundEmotes  = guildEmotes.Where(emote => emote is null).ToList();

            if (notFoundEmotes.Any())
            {
                await ReplyAsync($"Not found emotes: {string.Join(", ", notFoundEmotes)}");
                return;
            }

            var foundEmotes = guildEmotes.Where(x => x != null);

            await ReplyAsync(string.Join("", foundEmotes.Select(e => e!.ToString())));
        }
    }
}