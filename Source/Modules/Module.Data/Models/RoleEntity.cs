using System;
using Discord;
using MongoDB.Bson.Serialization.Attributes;

namespace Module.Data.Models
{
    public class RoleEntity : EntityBase<ulong>
    {
        public RoleEntity(IRole discordRole)
        {
            GuildDiscordId = discordRole.Guild.Id;
            IsHoisted = discordRole.IsHoisted;
            IsManaged = discordRole.IsManaged;
            IsMentionable = discordRole.IsMentionable;
            Name = discordRole.Name;
            Position = discordRole.Position;
            DiscordId = discordRole.Id;
            CreatedAt = discordRole.CreatedAt;
            Mention = discordRole.Mention;

            Id = DiscordId;
        }

        [BsonElement] public ulong DiscordId { get; set; }

        [BsonElement] public bool IsHoisted { get; set; }

        [BsonElement] public bool IsManaged { get; set; }

        [BsonElement] public bool IsMentionable { get; set; }

        [BsonElement] public string Name { get; set; }

        [BsonElement] public int Position { get; set; }

        [BsonElement] public ulong GuildDiscordId { get; set; }

        [BsonElement] public DateTimeOffset CreatedAt { get; set; }

        [BsonElement] public string Mention { get; set; }
    }
}