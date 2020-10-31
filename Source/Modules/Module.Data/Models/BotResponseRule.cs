using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Module.Data.Models
{
    public class BotResponseRule : BotMessageRuleBase
    {
        [BsonElement] public string RespondWith { get; set; } = "";
    }
}