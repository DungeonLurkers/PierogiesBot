using ReactiveUI;

namespace PierogiesBot.Manager.ViewModels
{
    public class CreateResponseRuleViewModel : ReactiveObject, IRoutableViewModel
    {
        public CreateResponseRuleViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
        }

        public string? UrlPathSegment { get; } = "createResponseRule";
        public IScreen HostScreen { get; }
    }
}