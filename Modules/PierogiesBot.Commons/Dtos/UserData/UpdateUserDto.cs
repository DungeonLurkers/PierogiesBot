using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PierogiesBot.Commons.Dtos.UserData
{
    public record UpdateUserDto(string UserName, string Email, IEnumerable<string> Roles);
}