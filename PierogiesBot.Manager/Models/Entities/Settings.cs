using System;

namespace PierogiesBot.Manager.Models.Entities
{
    public class Settings : EntityBase
    {
        public Settings(string currentUserName, string apiToken) : this(Guid.Empty, currentUserName, apiToken)
        {
            
        }

        public Settings(Guid id, string currentUserName, string apiToken) : base(id)
        {
            CurrentUserName = currentUserName;
            ApiToken = apiToken;
        }

        public string CurrentUserName { get; set; }
        public string ApiToken { get; set; }

        public void Deconstruct(out Guid id, out string currentUserName, out string apiToken)
        {
            id = Id;
            currentUserName = CurrentUserName;
            apiToken = ApiToken;
        }
    }
}