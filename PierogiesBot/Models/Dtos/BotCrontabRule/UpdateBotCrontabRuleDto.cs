namespace PierogiesBot.Models.Dtos.BotCrontabRule
{
    public record UpdateBotCrontabRuleDto(bool IsEmoji, string Crontab, ulong ChannelId, string? ReplyMessage, string? ReplyEmoji);
}