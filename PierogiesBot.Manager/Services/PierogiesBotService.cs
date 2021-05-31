using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.UserData;
using PierogiesBot.Commons.RestClient;
using PierogiesBot.Manager.Models.Messages;
using ReactiveUI;
using RestEase;

namespace PierogiesBot.Manager.Services
{
    public class PierogiesBotService : IPierogiesBotService
    {
        private readonly ILogger<PierogiesBotService> _logger;
        private readonly IMessageBus _messageBus;
        private readonly IPierogiesBotApi _api;
        public bool IsAuthenticated => Token is not null;
        public string? Token { get; private set; }

        public PierogiesBotService(ILogger<PierogiesBotService> logger, IMessageBus messageBus, IPierogiesBotApi api)
        {
            _logger = logger;
            _messageBus = messageBus;
            _api = api;
        }
        public async Task<bool> Authenticate(string userName, SecureString password)
        {
            _logger.LogInformation("Authenticating as {0}", userName);
            try
            {
                var credentials = new NetworkCredential(userName, password);
                var authResponse = await _api.Authenticate(new AuthenticateRequest(credentials.UserName, credentials.Password));

                Token = authResponse.Token;
                _api.AuthenticationHeaderValue = AuthenticationHeaderValue.Parse($"Bearer {Token}");
                _logger.LogInformation("Authenticated as {0}", authResponse.UserName);
            }
            catch (ApiException e)
            {
                _logger.LogError(e, "ApiException while authenticating!");
                return false;
            }

            return false;
        }

        private async Task<T?> Request<T>(Func<IPierogiesBotApi, Task<T>> func)
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
                    _messageBus.SendMessage(new NeedsUserLogin(), nameof(IPierogiesBotService));
                    
                }
                else
                {
                    _logger.LogError(e, "ApiException while making request");
                }
            }

            return default;
        }
    }
}