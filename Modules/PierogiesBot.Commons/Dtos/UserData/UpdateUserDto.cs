using System.Collections.Generic;

namespace PierogiesBot.Commons.Dtos.UserData
{
    public record UpdateUserDto(string UserName, string Email, IEnumerable<string> Roles);
}