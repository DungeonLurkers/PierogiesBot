using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.GrainsInterfaces.Data;

namespace PierogiesBot.Grains.Data
{
    public class BotResponseRuleGrain : EntityGrainBase<BotResponseRule>, IBotResponseRuleGrain
    {
        
        public BotResponseRuleGrain(IRepository<BotResponseRule> repository, IMapper mapper) : base(repository, mapper)
        {
        }
        
        public async Task<GetBotResponseRuleDto?> FindById(string id) => await FindById<GetBotResponseRuleDto>(id);

        public async Task<IEnumerable<GetBotResponseRuleDto>> Find() => await Find<GetBotResponseRuleDto>();

        public async Task<string> Create(CreateBotResponseRuleDto ruleDto) => await base.Create(ruleDto);

        public async Task<string> Update(string id, UpdateBotResponseRuleDto ruleDto) => await base.Update(id, ruleDto);
    }
}