using MongoDB.Bson;

namespace PierogiesBot.Data.Models
{
    public record GuildSettings(string Id, ulong GuildId, string GuildTimeZone, ulong GuildMuteRoleId) : EntityBase(Id)
    {
        public GuildSettings(ulong guildId, string guildTimeZone, ulong guildMuteRoleId)
            : this(ObjectId.GenerateNewId().ToString(), guildId, guildTimeZone, guildMuteRoleId)
        {
        }
    }
}