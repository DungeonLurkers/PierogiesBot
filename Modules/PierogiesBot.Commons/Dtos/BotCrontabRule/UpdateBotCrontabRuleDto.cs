using System.Collections.Generic;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Commons.Dtos.BotCrontabRule
{
    public record UpdateBotCrontabRuleDto(bool IsEmoji, string Crontab, IEnumerable<string> ReplyMessages,
        IEnumerable<string> ReplyEmojis, ResponseMode ResponseMode);
}