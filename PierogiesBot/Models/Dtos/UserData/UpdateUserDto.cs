using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PierogiesBot.Models.Dtos.UserData
{
    public record UpdateUserDto([Required] string UserName, [Required] string Email, IEnumerable<string> Roles);
}