using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Module.Data.Models
{
    public class BotResponseRule : EntityBase<Guid>
    {
        [BsonElement]
        public string RespondTo { get; set; }
        [BsonElement]
        public string RespondWith { get; set; }
        [BsonElement]
        public bool IsRespondToRegex { get; set; }
        [BsonElement]
        public bool IsRespondOnContains { get; set; }
        [BsonElement]
        public StringComparison StringComparison { get; set; }

        public BotResponseRule()
        {
            RespondTo = "";
            RespondWith = "";
            IsRespondToRegex = false;
        }
    }
}