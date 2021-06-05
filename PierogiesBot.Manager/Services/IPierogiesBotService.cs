using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Commons.Dtos.UserData;
using RestEase;

namespace PierogiesBot.Manager.Services
{
    public interface IPierogiesBotService
    {
        Task<bool> CheckIsAuthenticated();
        string? Token {get;}

        Task<bool> Authenticate(string userName = "", SecureString? password = null, bool renewToken=false);

        Task<GetUserDto?> GetUserData(string userName);
        
        Task<IEnumerable<GetBotResponseRuleDto>?> GetBotResponseRules();
        Task<IEnumerable<GetBotReactRuleDto>?> GetBotReactRules();
        Task<IEnumerable<GetBotCrontabRuleDto>?> GetBotCrontabRules();
    }
}