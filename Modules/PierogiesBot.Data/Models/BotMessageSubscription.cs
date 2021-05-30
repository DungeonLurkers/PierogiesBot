
using MongoDB.Bson;

namespace PierogiesBot.Data.Models
{
    public record BotMessageSubscription(string Id, ulong GuildId, ulong ChannelId) : EntityBase(Id)
    {
        public BotMessageSubscription(ulong guildId, ulong channelId) : this(ObjectId.GenerateNewId().ToString(), guildId, channelId)
        {
            
        }
    }
}