using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Http.Abstractions;
using PierogiesBot.Commons.Dtos.BotResponseRule;

namespace PierogiesBot.GrainsInterfaces.Data
{
    public interface IBotResponseRuleGrain
    {
        Task<GetBotResponseRuleDto?> FindById(string id);
        
        Task<IEnumerable<GetBotResponseRuleDto>> Find();

        Task<string> Create([FromBody] CreateBotResponseRuleDto ruleDto);

        Task<string> Update(string id,[FromBody] UpdateBotResponseRuleDto ruleDto);

        Task<string> Delete(string id);
    }
}