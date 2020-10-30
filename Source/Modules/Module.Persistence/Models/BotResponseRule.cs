using System;
using Module.Data.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Models
{
    public class BotResponseRule : EntityBase<Guid>
    {
        [BsonElement]
        public string RespondTo { get; set; }
        [BsonElement]
        public string RespondWith { get; set; }
        [BsonElement]
        public bool IsRespondToRegex { get; set; }

        public BotResponseRule()
        {
            RespondTo = "";
            RespondWith = "";
            IsRespondToRegex = false;
        }
    }
}