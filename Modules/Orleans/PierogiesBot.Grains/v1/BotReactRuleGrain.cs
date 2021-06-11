using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.GrainsInterfaces.v1;

namespace PierogiesBot.Grains.v1
{
    public class BotReactRuleGrain : EntityGrainBase<BotReactRule>, IBotReactRuleGrain
    {
        public BotReactRuleGrain(IRepository<BotReactRule> repository, IMapper mapper) : base(repository, mapper)
        {
        }
        
        public async Task<GetBotReactRuleDto?> FindById(string id) => await FindById<GetBotReactRuleDto>(id);

        public async Task<IEnumerable<GetBotReactRuleDto>> Find() => await Find<GetBotReactRuleDto>();

        public async Task<string> Create(CreateBotReactRuleDto ruleDto) => await Create<CreateBotReactRuleDto>(ruleDto);

        public async Task<string> Update(string id, UpdateBotReactRuleDto ruleDto) => await Update<UpdateBotReactRuleDto>(id, ruleDto);
    }
}