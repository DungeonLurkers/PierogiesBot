using System;
using MongoDB.Bson;

namespace PierogiesBot.Models
{
    public record BotCrontabRule(string Id, bool IsEmoji, string Crontab, ulong ChannelId, string? ReplyMessage,
        string? ReplyEmoji) : EntityBase(Id)
    {
        public BotCrontabRule(bool isEmoji, string crontab, ulong channelId, string? replyMessage,
            string? replyEmoji) : this(ObjectId.GenerateNewId().ToString(), isEmoji, crontab, channelId, replyMessage, replyEmoji)
        {
            
        }
    }
}