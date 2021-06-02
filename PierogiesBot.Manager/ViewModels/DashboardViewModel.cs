using ReactiveUI;

namespace PierogiesBot.Manager.ViewModels
{
    public class DashboardViewModel : ReactiveObject, IRoutableViewModel
    {
        public DashboardViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
        }

        public string? UrlPathSegment => "dashboard";
        public IScreen HostScreen { get; }
    }
}