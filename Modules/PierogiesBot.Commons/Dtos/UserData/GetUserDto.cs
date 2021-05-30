using System.Collections.Generic;

namespace PierogiesBot.Commons.Dtos.UserData
{
    public record GetUserDto(string Id, string UserName, IEnumerable<string> Roles);
}