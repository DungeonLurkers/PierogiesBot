using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace PierogiesBot.Data.Models
{
    public record BotCrontabRule(string Id, bool IsEmoji, string Crontab, IEnumerable<string> ReplyMessages,
        IEnumerable<string> ReplyEmoji, TimeZoneInfo TimeZoneInfo) : EntityBase(Id)
    {
        public BotCrontabRule(bool isEmoji, string crontab, IEnumerable<string> replyMessages,
            IEnumerable<string> replyEmojis, TimeZoneInfo timeZoneInfo) : 
            this(ObjectId.GenerateNewId().ToString(), 
                isEmoji, crontab, replyMessages,
                replyEmojis, timeZoneInfo)
        {
            
        }
    }
}