using System;

namespace PierogiesBot.Manager.Models.Entities
{
    public record Settings(string CurrentUserName, string ApiToken) : EntityBase(Guid.Parse("26F5C594-C32D-4A4E-9FAD-B678B710E97B"));
}