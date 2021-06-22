using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Http.Abstractions;
using PierogiesBot.Commons.Dtos.BotResponseRule;

namespace PierogiesBot.GrainsInterfaces.Data
{
    public interface IBotResponseRuleGrain : IEntityGrain
    {
        Task<GetBotResponseRuleDto?> FindById(string id);
        
        Task<IEnumerable<GetBotResponseRuleDto>> Find();

        Task<string> Create(CreateBotResponseRuleDto ruleDto);

        Task<string> Update(string id, UpdateBotResponseRuleDto ruleDto);
    }
}