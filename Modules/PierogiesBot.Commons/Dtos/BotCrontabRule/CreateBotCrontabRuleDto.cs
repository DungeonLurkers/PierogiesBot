using System;
using System.Collections.Generic;

namespace PierogiesBot.Commons.Dtos.BotCrontabRule
{
    public record CreateBotCrontabRuleDto(bool IsEmoji, string Crontab, IEnumerable<string> ReplyMessages,
        IEnumerable<string> ReplyEmojis, TimeZoneInfo TimeZoneInfo);
}