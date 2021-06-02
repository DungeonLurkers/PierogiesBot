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
        [Header("Authorization")]
        AuthenticationHeaderValue AuthenticationHeaderValue {get; set; }

        [Post("/api/User/auth")]
        public Task<AuthenticateResponse> Authenticate([Body] AuthenticateRequest request);

        [Get("/api/User/{id}")]
        Task<GetUserDto> GetUser([Path] string id);

        [Get("/api/User")]
        [AllowAnyStatusCode]
        Task<Response<string>> Ping();

        [Get("/api/BotResponseRule")]
        Task<IEnumerable<GetBotResponseRuleDto>> GetBotResponseRules();

        [Get("/api/BotReactRule")]
        Task<IEnumerable<GetBotReactRuleDto>> GetBotReactRules();
        
        [Get("/api/BotCrontabRule")]
        Task<IEnumerable<GetBotCrontabRuleDto>> GetBotCrontabRules();
    }
}