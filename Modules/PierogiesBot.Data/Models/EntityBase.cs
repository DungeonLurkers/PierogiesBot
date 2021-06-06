using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PierogiesBot.Data.Models
{
    public record EntityBase
    {
        public EntityBase() : this(ObjectId.GenerateNewId().ToString())
        {
        }

        public EntityBase(string id)
        {
            Id = id;
        }

        [BsonId] public string Id { get; init; }
    }
}