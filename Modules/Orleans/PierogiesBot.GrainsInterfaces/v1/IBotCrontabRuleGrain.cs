using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Http.Abstractions;
using PierogiesBot.Commons.Dtos.BotCrontabRule;

namespace PierogiesBot.GrainsInterfaces.v1
{
    [Route]
    public interface IBotCrontabRuleGrain : IGrainWithStringKey
    {
        [HttpGet]
        [Authorize(Roles="user,admin")]
        Task<GetBotCrontabRuleDto?> FindById(string id);
        
        [HttpGet]
        [Authorize(Roles="user,admin")]
        Task<IEnumerable<GetBotCrontabRuleDto>> Find();

        [HttpPost]
        [Authorize(Roles="admin")]
        Task<string> Create([FromBody] CreateBotCrontabRuleDto ruleDto);

        [HttpPut]
        [Authorize(Roles="admin")]
        Task<string> Update(string id,[FromBody] UpdateBotCrontabRuleDto ruleDto);

        [HttpDelete]
        [Authorize(Roles="admin")]
        Task<string> Delete(string id);

    }
}