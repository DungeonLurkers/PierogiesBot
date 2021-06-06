using System.Collections.Generic;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Commons.Dtos.BotCrontabRule
{
    public record CreateBotCrontabRuleDto(bool IsEmoji, string Crontab, IEnumerable<string> ReplyMessages,
        IEnumerable<string> ReplyEmojis, ResponseMode ResponseMode) : ICreateRuleDto;
}