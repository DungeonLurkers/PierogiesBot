using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;
using PierogiesBot.Manager.Models.Messages;
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
        private readonly IMessageBus _messageBus;
        private readonly ISettingsService _settingsService;

        public LoginViewModel(ILogger<LoginViewModel> logger, IPierogiesBotService botService, IScreen hostScreen,
            INavigationService navigationService, IMessageBus messageBus, ISettingsService settingsService)
        {
            _logger = logger;
            _botService = botService;
            _navigationService = navigationService;
            _messageBus = messageBus;
            _settingsService = settingsService;

            HostScreen = hostScreen;
            SignInCommand = ReactiveCommand.CreateFromTask<(string, SecureString)>(SignIn);
            SignInFromSettingsCommand = ReactiveCommand.CreateFromTask(Execute);

            SignInCommand.IsExecuting.ToPropertyEx(this, x => x.IsLogging);
            
        }

        private async Task Execute()
        {
            var settings = await _settingsService.Get();

            if (settings is null) return;

            if (await _botService.CheckIsAuthenticated()) _navigationService.NavigateToAndReset<DashboardViewModel>();
        }

        public ReactiveCommand<(string, SecureString), Unit> SignInCommand { get; }
        public ReactiveCommand<Unit, Unit> SignInFromSettingsCommand { get; }
        
        [ObservableAsProperty] public bool IsLogging { get; }

        public string? UrlPathSegment => "login";
        public IScreen HostScreen { get; }

        private async Task SignIn((string, SecureString) credentials)
        {
            var (userName, password) = credentials;

            try
            {
                var isSuccess = await _botService.Authenticate(userName, password);

                if (isSuccess) _navigationService.NavigateToAndReset<DashboardViewModel>();
                else
                {
                    MessageBox.Show("Authentication failed");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while authenticating as {0}", userName);
            }
        }
    }
}