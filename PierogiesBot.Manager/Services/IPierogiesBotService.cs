using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using PierogiesBot.Commons.Dtos;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Commons.Dtos.UserData;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Manager.Services
{
    public interface IPierogiesBotService
    {
        string? Token { get; }
        Task<bool> CheckIsAuthenticated();

        Task<bool> Authenticate(string userName = "", SecureString? password = null, bool renewToken = false);

        Task<GetUserDto?> GetUserData(string userName);

        Task<IEnumerable<GetBotResponseRuleDto>?> GetBotResponseRules();
        Task<IEnumerable<GetBotReactRuleDto>?> GetBotReactRules();
        Task<IEnumerable<GetBotCrontabRuleDto>?> GetBotCrontabRules();

        Task CreateBotResponseRule(bool shouldTriggerOnContains, bool isTriggerTextRegex,
            StringComparison stringComparison, string triggerText, IEnumerable<string> responses,
            ResponseMode responseMode);

        Task UploadRule<TRule>(TRule rule) where TRule : ICreateRuleDto;
    }
}