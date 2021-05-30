using System.Collections.Generic;

namespace PierogiesBot.Commons.Dtos.BotCrontabRule
{
    public record UpdateBotCrontabRuleDto(bool IsEmoji, string Crontab, IEnumerable<string> ReplyMessages,
        IEnumerable<string> ReplyEmojis);
}