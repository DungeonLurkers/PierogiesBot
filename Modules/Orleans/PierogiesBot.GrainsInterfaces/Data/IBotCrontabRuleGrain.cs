using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Http.Abstractions;
using PierogiesBot.Commons.Dtos.BotCrontabRule;

namespace PierogiesBot.GrainsInterfaces.Data
{
    public interface IBotCrontabRuleGrain : IEntityGrain
    {
        Task<GetBotCrontabRuleDto?> FindById(string id);
        
        Task<IEnumerable<GetBotCrontabRuleDto>> Find();

        Task<string> Create(CreateBotCrontabRuleDto ruleDto);

        Task<string> Update(string id, UpdateBotCrontabRuleDto ruleDto);
    }
}