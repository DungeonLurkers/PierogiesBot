using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Http.Abstractions;
using PierogiesBot.Commons.Dtos.BotReactRule;

namespace PierogiesBot.GrainsInterfaces.v1
{
    [Route]
    public interface IBotReactRuleGrain
    {
        [HttpGet]
        [Authorize(Roles="user,admin")]
        Task<GetBotReactRuleDto?> FindById(string id);
        
        [HttpGet]
        [Authorize(Roles="user,admin")]
        Task<IEnumerable<GetBotReactRuleDto>> Find();

        [HttpPost]
        [Authorize(Roles="admin")]
        Task<string> Create([FromBody] CreateBotReactRuleDto ruleDto);

        [HttpPut]
        [Authorize(Roles="admin")]
        Task<string> Update(string id,[FromBody] UpdateBotReactRuleDto ruleDto);

        [HttpDelete]
        [Authorize(Roles="admin")]
        Task<string> Delete(string id);
    }
}