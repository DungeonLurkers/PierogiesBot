using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PierogiesBot.Discord.Modules
{
    [Group("emoji")]
    public class EmojiCommandModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("Sends a single message containing specified emojis")]
        public async Task Emoji([Summary("Emojis")] params string[] emojis)
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