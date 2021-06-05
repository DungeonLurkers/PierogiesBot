using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Commons.Dtos.UserData;
using PierogiesBot.Commons.Enums;
using PierogiesBot.Commons.RestClient;
using PierogiesBot.Manager.Models.Messages;
using ReactiveUI;
using RestEase;

namespace PierogiesBot.Manager.Services
{
    public class PierogiesBotService : IPierogiesBotService
    {
        private readonly IPierogiesBotApi _api;
        private readonly ILogger<PierogiesBotService> _logger;
        private readonly IMessageBus _messageBus;
        private readonly ISettingsService _settingsService;

        public PierogiesBotService(ISettingsService settingsService, ILogger<PierogiesBotService> logger,
            IMessageBus messageBus, IPierogiesBotApi api)
        {
            _settingsService = settingsService;
            _logger = logger;
            _messageBus = messageBus;
            _api = api;
        }

        public async Task<bool> CheckIsAuthenticated()
        {
            var result = await Request(api => api.Ping());

            if (result is null) return false;

            return result.ResponseMessage.StatusCode != HttpStatusCode.Unauthorized;
        }

        public string? Token { get; private set; }

        public async Task<bool> Authenticate(string userName = "", SecureString? password = null, bool renewToken = false)
        {
            _logger.LogInformation("Authenticating as {0}", userName);
            try
            {
                var settings = await _settingsService.Get();

                if (settings is not null && settings.ApiToken is not "" && settings.CurrentUserName is not "")
                {
                    if (!renewToken)
                    {
                        _logger.LogDebug("Trying old token...");
                        if (!await CheckIsAuthenticated())
                        {
                            _logger.LogDebug("Old token is invalid! Trying to renew token...");
                            if (password is not null)
                                return await Authenticate(userName, password, true);
                            else
                                return false;
                        }

                        if (!string.IsNullOrEmpty(Token)) return true;
                    }
                }

                var credentials = new NetworkCredential(userName, password);
                var authResponse =
                    await _api.Authenticate(new AuthenticateRequest(credentials.UserName, credentials.Password));

                if (authResponse?.Token is null) return false;
                Token = authResponse.Token;
                await _settingsService.Set(userName, Token);

                _api.AuthenticationHeaderValue = AuthenticationHeaderValue.Parse($"Bearer {Token}");

                _logger.LogInformation("Authenticated as {0}", userName);

                return true;
            }
            catch (ApiException e)
            {
                _logger.LogError(e, "ApiException while authenticating!");
                if (e.StatusCode != HttpStatusCode.Unauthorized) return false;
                _logger.LogInformation("Refreshing access token");
                await Authenticate(userName, password, true);

                return false;
            }
        }

        public async Task<GetUserDto?> GetUserData(string userName)
        {
            return await Request(async api =>
                await api.GetUser(userName));
        }

        public async Task<IEnumerable<GetBotResponseRuleDto>?> GetBotResponseRules()
        {
            return await Request(async api => await api.GetBotResponseRules());
        }

        public async Task<IEnumerable<GetBotReactRuleDto>?> GetBotReactRules()
        {
            return await Request(async api => await api.GetBotReactRules());
        }

        public async Task<IEnumerable<GetBotCrontabRuleDto>?> GetBotCrontabRules()
        {
            return await Request(async api => await api.GetBotCrontabRules());
        }

        public async Task CreateBotResponseRule(bool shouldTriggerOnContains, bool isTriggerTextRegex,
            StringComparison stringComparison, string triggerText, IEnumerable<string> responses,
            ResponseMode responseMode)
        {
            await Request(async api => await api.CreateBotResponseRule(new CreateBotResponseRuleDto(responseMode, responses, triggerText, stringComparison, isTriggerTextRegex, shouldTriggerOnContains)));
        }

        private async Task<T?> Request<T>(Func<IPierogiesBotApi, Task<T>> func)
        {
            try
            {
                return await func(_api);
            }
            catch (ApiException e)
            {
                if (e.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogInformation("Request needs authentication, but current credentials don't work");
                    _logger.LogInformation("Requesting user login");
                    _messageBus.SendMessage(new NeedsUserLogin());
                }
                else
                {
                    _logger.LogError(e, "ApiException while making request");
                }
            }

            return default;
        }
        
        private async Task Request(Func<IPierogiesBotApi, Task> func)
        {
            try
            {
                await func(_api);
            }
            catch (ApiException e)
            {
                if (e.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogInformation("Request needs authentication, but current credentials don't work");
                    _logger.LogInformation("Requesting user login");
                    _messageBus.SendMessage(new NeedsUserLogin());
                }
                else
                {
                    _logger.LogError(e, "ApiException while making request");
                }
            }
        }
    }
}