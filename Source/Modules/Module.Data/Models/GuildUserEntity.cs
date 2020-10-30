using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using MongoDB.Bson.Serialization.Attributes;

namespace Module.Data.Models
{
    public class GuildUserEntity : EntityBase<ulong>
    {
        [BsonElement] public ulong DiscordId { get; set; }
        [BsonElement] public string Nickname { get; set; }
        [BsonElement] public string Username { get; set; }
        [BsonElement] public ulong GuildDiscordId { get; set; }
        [BsonElement] public List<ulong> RolesDiscordIds { get; set; }
        [BsonElement] public DateTimeOffset? JoinedAt { get; set; }
        [BsonElement] public bool IsBot { get; set; }
        public GuildUserEntity(IGuildUser user)
        {
            Id = user.Id;
            DiscordId = user.Id;
            Nickname = user.Nickname;
            GuildDiscordId = user.GuildId;
            RolesDiscordIds = user.RoleIds.ToList();
            Username = user.Username;
            JoinedAt = user.JoinedAt;
            IsBot = user.IsBot;
        }
    }
}