using System;
using System.Reactive;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PierogiesBot.Manager.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PierogiesBot.Manager.ViewModels
{
    public class LoginViewModel : ReactiveObject
    {
        private readonly ILogger<LoginViewModel> _logger;
        private readonly IPierogiesBotService _botService;
        
        public ReactiveCommand<(string, SecureString), Unit> SignInCommand { get; }

        public LoginViewModel(ILogger<LoginViewModel> logger, IPierogiesBotService botService)
        {
            _logger = logger;
            _botService = botService;
            SignInCommand = ReactiveCommand.CreateFromTask<(string, SecureString)>(SignIn);
        }

        private async Task<bool> SignIn((string, SecureString) credentials)
        {
            var (userName, password) = credentials;

            try
            {
                await _botService.Authenticate(userName, password);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while authenticating as {0}", userName);
            }

            return _botService.IsAuthenticated;
        }
    }
}