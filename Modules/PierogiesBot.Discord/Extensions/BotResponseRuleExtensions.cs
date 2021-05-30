using System;
using System.Text.RegularExpressions;
using PierogiesBot.Data.Models;

namespace PierogiesBot.Discord.Extensions
{
    public static class BotResponseRuleExtensions
    {
        private static Random _random = new();
        public static bool CanExecuteRule(this BotMessageRuleBase rule, string message)
        {
            var isRegex = rule.IsTriggerTextRegex;
            var triggerOnContains = rule.ShouldTriggerOnContains;
            
            return (isRegex, triggerOnContains) switch
            {
                (true, true) => ContainsMatchRegex(message, rule),
                (true, false) => IsMatchRegex(message, rule),
                (false, false) => IsMatchText(message, rule),
                (false, true) => ContainsText(message, rule)
            };
        }

        private static bool ContainsMatchRegex(string message, BotMessageRuleBase rule) => Regex.IsMatch(message, rule.TriggerText);

        private static bool IsMatchRegex(string message, BotMessageRuleBase rule) => Regex.IsMatch(message, $"^{rule.TriggerText}$");

        private static bool IsMatchText(string message, BotMessageRuleBase rule) => message.Equals(rule.TriggerText, rule.StringComparison);

        private static bool ContainsText(string message, BotMessageRuleBase rule) => message.Contains(rule.TriggerText, rule.StringComparison);
    }
}