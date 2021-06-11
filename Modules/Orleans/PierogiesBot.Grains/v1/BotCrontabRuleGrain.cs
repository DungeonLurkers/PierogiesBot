using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Orleans;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.GrainsInterfaces.v1;

namespace PierogiesBot.Grains.v1
{
    public class BotCrontabRuleGrain : EntityGrainBase<BotCrontabRule>, IBotCrontabRuleGrain
    {
        public BotCrontabRuleGrain(IRepository<BotCrontabRule> repository, IMapper mapper) : base(repository, mapper)
        {
        }
        
        public async Task<GetBotCrontabRuleDto?> FindById(string id) => await FindById<GetBotCrontabRuleDto>(id);

        public async Task<IEnumerable<GetBotCrontabRuleDto>> Find() => await Find<GetBotCrontabRuleDto>();

        public async Task<string> Create(CreateBotCrontabRuleDto ruleDto) => await Create<CreateBotCrontabRuleDto>(ruleDto);

        public async Task<string> Update(string id, UpdateBotCrontabRuleDto ruleDto) => await Update<UpdateBotCrontabRuleDto>(id, ruleDto);
    }
}