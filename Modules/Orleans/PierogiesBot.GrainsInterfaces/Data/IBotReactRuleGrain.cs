using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Http.Abstractions;
using PierogiesBot.Commons.Dtos.BotReactRule;

namespace PierogiesBot.GrainsInterfaces.Data
{
    public interface IBotReactRuleGrain : IEntityGrain
    {
        Task<GetBotReactRuleDto?> FindById(string id);
        
        Task<IEnumerable<GetBotReactRuleDto>> Find();

        Task<string> Create(CreateBotReactRuleDto ruleDto);

        Task<string> Update(string id, UpdateBotReactRuleDto ruleDto);

        Task<string> Delete(string id);
    }
}