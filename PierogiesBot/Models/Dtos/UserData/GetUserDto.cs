using System.Collections.Generic;

namespace PierogiesBot.Models.Dtos.UserData
{
    public record GetUserDto(string Id, string UserName, IEnumerable<string> Roles);
}