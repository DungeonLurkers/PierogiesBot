using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.GrainsInterfaces.Data;

namespace PierogiesBot.Grains.Data
{
    public class BotCrontabRuleGrain : EntityGrainBase<BotCrontabRule>, IBotCrontabRuleGrain
    {
        public BotCrontabRuleGrain(IRepository<BotCrontabRule> repository, IMapper mapper) : base(repository, mapper)
        {
        }
        
        public new async Task<GetBotCrontabRuleDto?> FindById(string id) => await FindById<GetBotCrontabRuleDto>(id);

        public new async Task<IEnumerable<GetBotCrontabRuleDto>> Find() => await Find<GetBotCrontabRuleDto>();

        public async Task<string> Create(CreateBotCrontabRuleDto ruleDto) => await base.Create(ruleDto);

        public async Task<string> Update(string id, UpdateBotCrontabRuleDto ruleDto) => await base.Update(id, ruleDto);
    }
}