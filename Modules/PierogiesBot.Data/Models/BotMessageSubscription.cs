
using MongoDB.Bson;

namespace PierogiesBot.Data.Models
{
    public record BotMessageSubscription(string Id, ulong GuildId, ulong ChannelId, SubscriptionType SubscriptionType) : EntityBase(Id)
    {
        public BotMessageSubscription(ulong guildId, ulong channelId, SubscriptionType subscriptionType) : this(ObjectId.GenerateNewId().ToString(), guildId, channelId, subscriptionType)
        {
            
        }
    }

    public enum SubscriptionType
    {
        Empty,
        Responses,
        Crontab
    }
}