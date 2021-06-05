using System.Reactive;
using System.Threading.Tasks;
using PierogiesBot.Manager.Services;
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

            GoToCreateResponseRuleCommand = ReactiveCommand.Create(Execute);
        }

        private void Execute()
        {
            _navigationService.NavigateTo<CreateResponseRuleViewModel>();
        }

        public string? UrlPathSegment => "dashboard";
        public IScreen HostScreen { get; }
        
        public ReactiveCommand<Unit, Unit> GoToCreateResponseRuleCommand { get; private set; }
    }
}