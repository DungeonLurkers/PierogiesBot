using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Module.Data.Models
{
    public abstract class EntityBase<TId>
    {
        [BsonId]
        public TId Id { get; set; }
        
        [BsonExtraElements]
        public BsonDocument ExtraElements { get; set; }
    }
}