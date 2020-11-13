using System;
using System.Collections.Generic;
using System.Linq;
using Module.Data.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Module.Data.Models
{
    public class QuestionEntity : EntityBase<Guid>
    {
        [BsonElement] public ulong OwnerSnowflakeId { get; set; } = 0ul;
        [BsonElement] public string QuestionContent { get; set; } = "";
        [BsonElement] public ICollection<string> Answers { get; set; } = new List<string>();
        [BsonElement] public ISet<long> VotingUserIds { get; set; } = new HashSet<long>();
        [BsonElement] public QuestionState QuestionState { get; set; } = QuestionState.Building;
        [BsonElement] public bool IsOpen { get; set; } = false;
        [BsonElement] public bool IsMultipleChoice { get; set; } = false;
    }
}