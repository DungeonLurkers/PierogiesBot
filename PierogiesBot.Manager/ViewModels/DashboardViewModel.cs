using System.Reactive;
using PierogiesBot.Manager.Services;
using PierogiesBot.Manager.ViewModels.CreateRules;
using ReactiveUI;

namespace PierogiesBot.Manager.ViewModels
{
    public class DashboardViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly INavigationService _navigationService;

        public DashboardViewModel(IScreen hostScreen, INavigationService navigationService)
        {
            _navigationService = navigationService;
            HostScreen = hostScreen;

            GoToCreateResponseRuleCommand =
                ReactiveCommand.Create(() => _navigationService.NavigateTo<CreateResponseRuleViewModel>());
            GoToCreateReactionRuleCommand =
                ReactiveCommand.Create(() => _navigationService.NavigateTo<CreateReactRuleViewModel>());
            GoToCreateCrontabRuleCommand =
                ReactiveCommand.Create(() => _navigationService.NavigateTo<CreateCrontabRuleViewModel>());
        }

        public string? UrlPathSegment => "dashboard";
        public IScreen HostScreen { get; }

        public ReactiveCommand<Unit, Unit> GoToCreateResponseRuleCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToCreateReactionRuleCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToCreateCrontabRuleCommand { get; }
    }
}