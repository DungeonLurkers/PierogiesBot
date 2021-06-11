using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Commons.Dtos.UserData;
using RestEase;

namespace PierogiesBot.Commons.RestClient
{
    public interface IPierogiesBotApi
    {
        [Header("Authorization")] AuthenticationHeaderValue AuthenticationHeaderValue { get; set; }

        [Post("/api/User/auth")]
        public Task<AuthenticateResponse> Authenticate([Body] AuthenticateRequest request);

        [Get("/api/User/{id}")]
        Task<GetUserDto> GetUser([Path] string id);

        [Get("/api/User")]
        [AllowAnyStatusCode]
        Task<Response<string>> Ping();

        [Get("/Grains/PierogiesBot.GrainsInterfaces.v1.IBotResponseRuleGrain/a/Find")]
        Task<IEnumerable<GetBotResponseRuleDto>> GetBotResponseRules();

        [Get("/Grains/PierogiesBot.GrainsInterfaces.v1.IBotReactRuleGrain/a/Find")]
        Task<IEnumerable<GetBotReactRuleDto>> GetBotReactRules();

        [Get("/Grains/PierogiesBot.GrainsInterfaces.v1.IBotCrontabRuleGrain/a/Find")]
        Task<IEnumerable<GetBotCrontabRuleDto>> GetBotCrontabRules();

        [Post("/Grains/PierogiesBot.GrainsInterfaces.v1.IBotResponseRuleGrain/a/Create")]
        Task CreateBotResponseRule([Body] CreateBotResponseRuleDto responseRuleDto);

        [Post("/Grains/PierogiesBot.GrainsInterfaces.v1.IBotReactRuleGrain/a/Create")]
        Task CreateBotReactRule([Body] CreateBotReactRuleDto reactRuleDto);

        [Post("/Grains/PierogiesBot.GrainsInterfaces.v1.IBotCrontabRuleGrain/a/Create")]
        Task CreateBotCrontabRule([Body] CreateBotCrontabRuleDto crontabRuleDto);
    }
}