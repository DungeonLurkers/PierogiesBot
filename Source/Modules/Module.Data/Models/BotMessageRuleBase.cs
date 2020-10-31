using System;

namespace Module.Data.Models
{
    public abstract class BotMessageRuleBase : EntityBase<Guid>
    {
        public string TriggerText { get; set; } = "";
        public StringComparison StringComparison { get; set; } = StringComparison.InvariantCultureIgnoreCase;
        public bool IsTriggerTextRegex { get; set; } = false;
        public bool ShouldTriggerOnContains { get; set; } = false;
    }
}