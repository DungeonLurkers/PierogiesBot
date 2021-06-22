using System;
using System.Collections.Generic;

namespace PierogiesBot.Commons.Dtos.Mute
{
    public record GetMuteDto(ulong DiscordUserId, ulong DiscordGuildId, DateTimeOffset Until, string Reason, List<ulong> RolesIds) : IFindEntityDto;
}