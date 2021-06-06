using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PierogiesBot.Manager.Models.Messages;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;

namespace PierogiesBot.Manager.Services
{
    public class UserLoginHostedService : IHostedService
    {
        private readonly ILogger<UserLoginHostedService> _logger;
        private readonly IFactory<IViewFor<LoginViewModel>> _loginViewFactory;
        private readonly IMessageBus _messageBus;
        private readonly INavigationService _navigationService;
        private IDisposable? _subscription;

        public UserLoginHostedService(ILogger<UserLoginHostedService> logger, IMessageBus messageBus,
            INavigationService navigationService)
        {
            _logger = logger;
            _messageBus = messageBus;
            _navigationService = navigationService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting user login hosted service");

            _subscription = _messageBus
                .ListenIncludeLatest<NeedsUserLogin>()
                .Where(x => x is { })
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(_ => NavigateToLogin())
                .Subscribe();

            return Task.CompletedTask;
        }

        private void NavigateToLogin()
        {
            _logger.LogInformation("Showing login view");
            _navigationService.NavigateTo<LoginViewModel>();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping user login hosted service");
            _subscription?.Dispose();

            return Task.CompletedTask;
        }
    }
}