using MongoDB.Bson;
using PierogiesBot.Data.Enums;

namespace PierogiesBot.Data.Models
{
    public record BotMessageSubscription
        (string Id, ulong GuildId, ulong ChannelId, SubscriptionType SubscriptionType) : EntityBase(Id)
    {
        public BotMessageSubscription(ulong guildId, ulong channelId, SubscriptionType subscriptionType) : this(
            ObjectId.GenerateNewId().ToString(), guildId, channelId, subscriptionType)
        {
        }
    }
}