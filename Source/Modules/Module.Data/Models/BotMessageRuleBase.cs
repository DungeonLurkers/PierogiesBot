using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Module.Data.Models
{
    public abstract class BotMessageRuleBase : EntityBase<Guid>
    {
        public BotMessageRuleBase()
        {
            Id = Guid.NewGuid();
        }
        [BsonElement]
        public string TriggerText { get; set; } = "";
        [BsonElement]
        public StringComparison StringComparison { get; set; } = StringComparison.InvariantCultureIgnoreCase;
        [BsonElement]
        public bool IsTriggerTextRegex { get; set; } = false;
        [BsonElement]
        public bool ShouldTriggerOnContains { get; set; } = false;
    }
}