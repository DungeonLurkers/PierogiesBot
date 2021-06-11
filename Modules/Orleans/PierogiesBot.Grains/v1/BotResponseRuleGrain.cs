using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Orleans;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.GrainsInterfaces.v1;

namespace PierogiesBot.Grains.v1
{
    public class BotResponseRuleGrain : EntityGrainBase<BotResponseRule>, IBotResponseRuleGrain
    {
        
        public BotResponseRuleGrain(IRepository<BotResponseRule> repository, IMapper mapper) : base(repository, mapper)
        {
        }
        
        public async Task<GetBotResponseRuleDto?> FindById(string id) => await FindById<GetBotResponseRuleDto>(id);

        public async Task<IEnumerable<GetBotResponseRuleDto>> Find() => await Find<GetBotResponseRuleDto>();

        public async Task<string> Create(CreateBotResponseRuleDto ruleDto) => await Create<CreateBotResponseRuleDto>(ruleDto);

        public async Task<string> Update(string id, UpdateBotResponseRuleDto ruleDto) => await Update<UpdateBotResponseRuleDto>(id, ruleDto);
    }
}