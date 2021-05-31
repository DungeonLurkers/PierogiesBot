using System.Reactive;
using System.Threading.Tasks;
using PierogiesBot.Manager.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PierogiesBot.Manager.ViewModels
{
    public class UserProfileViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly ISettingsService _settingsService;
        
        [Reactive] public string UserName { get; set;}
        
        public ReactiveCommand<Unit, Unit> LoadCurrentUserData { get; private set; }

        public UserProfileViewModel(ISettingsService settingsService, IScreen hostScreen)
        {
            _settingsService = settingsService;
            HostScreen = hostScreen;
            UserName = "";
            
            LoadCurrentUserData = ReactiveCommand.CreateFromTask(async () =>
            {
                await Initialize();
            });
        }

        private async Task Initialize()
        {
            var settings = await _settingsService.Get();
            if (settings is not null) UserName = settings.CurrentUserName;
        }

        public string? UrlPathSegment => $"User/{UserName}";
        public IScreen HostScreen { get; }
    }
}