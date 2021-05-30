using AspNetCore.Identity.MongoDB;

namespace PierogiesBot.Models
{
    public class AppUser : MongoIdentityUser
    {
        public ulong DiscordUserId { get; set; }
    }
}