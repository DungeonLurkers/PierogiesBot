using System;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using PierogiesBot.Manager.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PierogiesBot.Manager.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IPierogiesBotService _botService;

        public ReactiveCommand<Unit, Unit> CheckIsAuthenticated {get; private set;}

        public MainWindowViewModel(INavigationService navigationService, IPierogiesBotService botService)
        {
            _navigationService = navigationService;
            _botService = botService;

            Router = _navigationService.Router;

            CheckIsAuthenticated = ReactiveCommand.Create(Execute);
        }

        private void Execute()
        {
            if (_botService.IsAuthenticated) _navigationService.NavigateTo<UserProfileViewModel>();
            else _navigationService.NavigateTo<LoginViewModel>();
        }

        public RoutingState Router { get; }
    }
}