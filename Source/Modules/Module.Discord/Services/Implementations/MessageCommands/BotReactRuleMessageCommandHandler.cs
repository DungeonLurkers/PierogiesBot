using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Module.Data.Models;
using Module.Data.Storage;
using Module.Discord.Services.Definitions;

namespace Module.Discord.Services.Implementations.MessageCommands
{
    public class BotReactRuleMessageCommandHandler : BotMessageRuleMessageCommandHandlerBase<BotReactRule>
    {
        private readonly IDataSource<GuildEntity, ulong> _guildDataSource;
        private readonly IDiscordClient _discordClient;

        public BotReactRuleMessageCommandHandler(IDataSource<BotReactRule, Guid> rules,
            IDataSource<GuildEntity, ulong> guildDataSource,
            IDiscordClient discordClient) : base(rules)
        {
            _guildDataSource = guildDataSource;
            _discordClient = discordClient;
        }
        protected override async Task CommandAction(IMessage message, BotReactRule rule)
        {
            var guildEntity = _guildDataSource.GetAll().Single();
            var guild = await _discordClient.GetGuildAsync(guildEntity.DiscordId);

            var emotes = guild.Emotes;

            var reaction = emotes.FirstOrDefault(emote => emote.Name == rule.Reaction);

            if (reaction == null) return;

            await message.AddReactionAsync(reaction);
        }
    }
}