using System;
using System.Reactive;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PierogiesBot.Manager.Services;
using PierogiesBot.Manager.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PierogiesBot.Manager.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IPierogiesBotService _botService;
        private readonly ILogger<LoginViewModel> _logger;
        private readonly INavigationService _navigationService;

        public LoginViewModel(ILogger<LoginViewModel> logger, IPierogiesBotService botService, IScreen hostScreen,
            INavigationService navigationService)
        {
            _logger = logger;
            _botService = botService;
            _navigationService = navigationService;

            HostScreen = hostScreen;
            SignInCommand = ReactiveCommand.CreateFromTask<(string, SecureString)>(SignIn);

            SignInCommand.IsExecuting.ToPropertyEx(this, x => x.IsLogging);
        }

        public ReactiveCommand<(string, SecureString), Unit> SignInCommand { get; }
        
        [ObservableAsProperty] public bool IsLogging { get; }

        public string? UrlPathSegment => "login";
        public IScreen HostScreen { get; }

        private async Task SignIn((string, SecureString) credentials)
        {
            var (userName, password) = credentials;

            try
            {
                var isSuccess = await _botService.Authenticate(userName, password);

                if (isSuccess) _navigationService.NavigateToAndReset<UserProfileViewModel>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while authenticating as {0}", userName);
            }
        }
    }
}