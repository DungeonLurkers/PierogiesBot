using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using PierogiesBot.Commons.Dtos.Mute;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.GrainsInterfaces.Data;

namespace PierogiesBot.Grains.Data
{
    public class MuteGrain : EntityGrainBase<Mute>, IMuteGrain
    {
        public MuteGrain(IRepository<Mute> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public new Task<GetMuteDto?> FindById(string id) => base.FindById<GetMuteDto>(id);

        public new Task<IEnumerable<GetMuteDto>> Find() => base.Find<GetMuteDto>();

        public Task<string> Create(CreateMuteDto dto) => base.Create(dto);

        public Task<string> Update(string id, UpdateMuteDto dto) => base.Update(id, dto);

        public async Task<GetMuteDto?> FindByDiscordUserId(ulong id)
        {
             var entity = await Repository.GetByProperty(x => x.DiscordUserId, id);

             return entity is { } ? Mapper.Map<GetMuteDto>(entity) : null;
        }
    }
}