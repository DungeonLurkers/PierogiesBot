using System;
using MongoDB.Bson;

namespace PierogiesBot.Data.Models
{
    public record GuildSettings(string Id, ulong GuildId, string GuildTimeZone) : EntityBase(Id)
    {
        public GuildSettings(ulong guildId, string guildTimeZone) : this(ObjectId.GenerateNewId().ToString(), guildId, guildTimeZone)
        {
            
        }
    }
}