using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PierogiesBot.Commons.Dtos.UserData
{
    public record CreateUserDto([Required] string UserName, [Required] string Email, string Password, IEnumerable<string> Roles);
}