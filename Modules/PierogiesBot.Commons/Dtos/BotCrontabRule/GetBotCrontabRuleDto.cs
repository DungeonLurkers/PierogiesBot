﻿using System.Collections.Generic;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Commons.Dtos.BotCrontabRule
{
    public record GetBotCrontabRuleDto(string Id, bool IsEmoji, string Crontab, IEnumerable<string> ReplyMessages,
        IEnumerable<string> ReplyEmoji, ResponseMode ResponseMode) : BotCrontabRuleDtoBase, IFindEntityDto;
}