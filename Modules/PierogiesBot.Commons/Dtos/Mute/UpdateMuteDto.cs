using System;
using System.Collections.Generic;

namespace PierogiesBot.Commons.Dtos.Mute
{
    public record UpdateMuteDto(ulong DiscordUserId, ulong DiscordGuildId, DateTimeOffset Until, string Reason, List<ulong> RolesIds) : IUpdateEntityDto;
}