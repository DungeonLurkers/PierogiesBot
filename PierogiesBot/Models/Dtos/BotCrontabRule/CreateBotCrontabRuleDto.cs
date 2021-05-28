namespace PierogiesBot.Models.Dtos.BotCrontabRule
{
    public record CreateBotCrontabRuleDto(bool IsEmoji, string Crontab, ulong ChannelId, string? ReplyMessage,
        string? ReplyEmoji);
}