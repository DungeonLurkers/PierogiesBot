using System.Collections.Generic;
using MongoDB.Bson;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Data.Models
{
    public record BotCrontabRule(string Id, bool IsEmoji, string Crontab, IEnumerable<string> ReplyMessages,
        IEnumerable<string> ReplyEmoji, ResponseMode ResponseMode) : EntityBase(Id)
    {
        public BotCrontabRule(bool isEmoji, string crontab, IEnumerable<string> replyMessages,
            IEnumerable<string> replyEmojis, ResponseMode responseMode) :
            this(ObjectId.GenerateNewId().ToString(),
                isEmoji, crontab, replyMessages,
                replyEmojis, responseMode)
        {
        }
    }
}