using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using PierogiesBot.Commons.Dtos.Mute;

namespace PierogiesBot.GrainsInterfaces.Data
{
    public interface IMuteGrain : IEntityGrain
    {
        Task<GetMuteDto?> FindById(string id);
        
        Task<IEnumerable<GetMuteDto>> Find();

        Task<string> Create(CreateMuteDto ruleDto);

        Task<string> Update(string id, UpdateMuteDto ruleDto);
        
        Task<GetMuteDto?> FindByDiscordUserId(ulong id);
    }
}