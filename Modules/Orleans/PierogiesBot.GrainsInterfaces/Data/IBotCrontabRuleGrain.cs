using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Http.Abstractions;
using PierogiesBot.Commons.Dtos.BotCrontabRule;

namespace PierogiesBot.GrainsInterfaces.Data
{
    public interface IBotCrontabRuleGrain : IGrainWithStringKey
    {
        Task<GetBotCrontabRuleDto?> FindById(string id);
        
        Task<IEnumerable<GetBotCrontabRuleDto>> Find();

        Task<string> Create([FromBody] CreateBotCrontabRuleDto ruleDto);

        Task<string> Update(string id,[FromBody] UpdateBotCrontabRuleDto ruleDto);

        Task<string> Delete(string id);
    }
}