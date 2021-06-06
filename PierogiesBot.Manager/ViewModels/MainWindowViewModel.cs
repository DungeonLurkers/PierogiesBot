using System.Reactive;
using PierogiesBot.Manager.Services;
using ReactiveUI;

namespace PierogiesBot.Manager.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        private readonly IPierogiesBotService _botService;
        private readonly INavigationService _navigationService;

        public MainWindowViewModel(INavigationService navigationService, IPierogiesBotService botService)
        {
            _navigationService = navigationService;
            _botService = botService;

            Router = _navigationService.Router;

            CheckIsAuthenticated = ReactiveCommand.Create(Execute);
        }

        public ReactiveCommand<Unit, Unit> CheckIsAuthenticated { get; }

        public RoutingState Router { get; }

        private void Execute()
        {
            _navigationService.NavigateTo<LoginViewModel>();
        }
    }
}