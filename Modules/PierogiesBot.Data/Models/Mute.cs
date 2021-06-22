using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace PierogiesBot.Data.Models
{
    public record Mute(string Id, ulong DiscordUserId, ulong DiscordGuildId, DateTimeOffset Until, string Reason, List<ulong> RolesIds) : EntityBase(Id)
    {
        public Mute(ulong discordUserId, ulong discordGuildId, DateTimeOffset until, string reason, List<ulong> rolesIds)
        : this(ObjectId.GenerateNewId().ToString(), discordUserId, discordGuildId, until, reason, rolesIds)
        {

        }
    }
}