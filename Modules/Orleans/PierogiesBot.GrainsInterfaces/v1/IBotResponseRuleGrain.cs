using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Http.Abstractions;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Dtos.BotResponseRule;

namespace PierogiesBot.GrainsInterfaces.v1
{
    [Route]
    public interface IBotResponseRuleGrain
    {
        [HttpGet]
        [Authorize(Roles="user,admin")]
        Task<GetBotResponseRuleDto?> FindById(string id);
        
        [HttpGet]
        [Authorize(Roles="user,admin")]
        Task<IEnumerable<GetBotResponseRuleDto>> Find();

        [HttpPost]
        [Authorize(Roles="admin")]
        Task<string> Create([FromBody] CreateBotResponseRuleDto ruleDto);

        [HttpPut]
        [Authorize(Roles="admin")]
        Task<string> Update(string id,[FromBody] UpdateBotResponseRuleDto ruleDto);

        [HttpDelete]
        [Authorize(Roles="admin")]
        Task<string> Delete(string id);
    }
}