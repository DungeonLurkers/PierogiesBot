using System.Collections.Generic;
using System.Linq;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Manager.Models
{
    public record CrontabRuleModel(string Id, bool IsEmoji, string Crontab, IEnumerable<string> ReplyMessages,
        IEnumerable<string> ReplyEmoji, ResponseMode ResponseMode) : GetBotCrontabRuleDto(Id, IsEmoji, Crontab,
        ReplyMessages, ReplyEmoji, ResponseMode)
    {
        public string ResponsesAsString =>
                    ResponseMode == ResponseMode.First ? ReplyMessages.First() : $"'{string.Join(", ", ReplyMessages)}'";
        
        public string EmojisAsString =>
            ResponseMode == ResponseMode.First ? ReplyEmoji.First() : $"'{string.Join(", ", ReplyEmoji)}'";
    }
}