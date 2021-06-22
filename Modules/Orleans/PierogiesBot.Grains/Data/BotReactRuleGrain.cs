using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.GrainsInterfaces.Data;

namespace PierogiesBot.Grains.Data
{
    public class BotReactRuleGrain : EntityGrainBase<BotReactRule>, IBotReactRuleGrain
    {
        public BotReactRuleGrain(IRepository<BotReactRule> repository, IMapper mapper) : base(repository, mapper)
        {
        }
        
        public new async Task<GetBotReactRuleDto?> FindById(string id) => await FindById<GetBotReactRuleDto>(id);

        public new async Task<IEnumerable<GetBotReactRuleDto>> Find() => await Find<GetBotReactRuleDto>();

        public async Task<string> Create(CreateBotReactRuleDto ruleDto) => await base.Create(ruleDto);

        public async Task<string> Update(string id, UpdateBotReactRuleDto ruleDto) => await base.Update(id, ruleDto);
    }
}