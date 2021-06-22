using System.Threading.Tasks;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Data.Extensions
{
    public static class MuteRepositoryExtensions
    {
        public static Task<Mute?> FindByDiscordUserId(this IRepository<Mute> repository, ulong id) =>
            repository.GetByProperty(x => x.DiscordUserId, id);
    }
}