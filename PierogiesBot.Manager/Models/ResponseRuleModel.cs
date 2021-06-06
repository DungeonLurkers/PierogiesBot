using System;
using System.Collections.Generic;
using System.Linq;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Manager.Models
{
    public record ResponseRuleModel(string Id, ResponseMode ResponseMode, IEnumerable<string> Responses,
        string TriggerText,
        StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains) :
        GetBotResponseRuleDto(Id, ResponseMode, Responses, TriggerText, StringComparison, IsTriggerTextRegex,
            ShouldTriggerOnContains)
    {
        public string ResponsesAsString =>
            ResponseMode == ResponseMode.First ? Responses.First() : $"'{string.Join(", ", Responses)}'";
    }
}