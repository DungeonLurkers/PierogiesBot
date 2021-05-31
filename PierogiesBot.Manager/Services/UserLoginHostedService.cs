using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PierogiesBot.Manager.Models.Messages;
using PierogiesBot.Manager.ViewModels;
using PierogiesBot.Manager.Views;
using ReactiveUI;

namespace PierogiesBot.Manager.Services
{
    public class UserLoginHostedService : IHostedService
    {
        private readonly ILogger<UserLoginHostedService> _logger;
        private readonly IMessageBus _messageBus;
        private readonly IFactory<IViewFor<LoginViewModel>> _loginViewFactory;
        private IDisposable? _subscription;

        public UserLoginHostedService(ILogger<UserLoginHostedService> logger, IMessageBus messageBus, IFactory<IViewFor<LoginViewModel>> loginViewFactory)
        {
            _logger = logger;
            _messageBus = messageBus;
            _loginViewFactory = loginViewFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting user login hosted service");
            
            _subscription = _messageBus
                .ListenIncludeLatest<NeedsUserLogin>()
                .Where(x => x is { })
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(_ => ShowLoginView())
                .Subscribe();
            
            return Task.CompletedTask;
        }

        private void ShowLoginView()
        {
            _logger.LogInformation("Showing login view");
            var view = _loginViewFactory.Create();
            if (view is Window window) window.Show();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping user login hosted service");
            _subscription?.Dispose();

            return Task.CompletedTask;
        }
    }
}