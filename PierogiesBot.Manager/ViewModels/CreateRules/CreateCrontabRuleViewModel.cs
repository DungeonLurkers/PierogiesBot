using System.Collections.ObjectModel;
using System.Linq;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Enums;
using PierogiesBot.Manager.Models;
using PierogiesBot.Manager.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PierogiesBot.Manager.ViewModels.CreateRules
{
    public class CreateCrontabRuleViewModel : CreateRuleViewModelBase<CreateBotCrontabRuleDto>, IRoutableViewModel
    {
        public CreateCrontabRuleViewModel(IPierogiesBotService botService, ILogger<CreateCrontabRuleViewModel> logger,
            IScreen hostScreen) : base(botService, logger)
        {
            HostScreen = hostScreen;

            Responses = new ObservableCollectionExtended<Response>();
            Emojis = new ObservableCollectionExtended<Response>();

            Crontab = "* * * * * ?";
            ResponseMode = ResponseMode.First;
            IsEmoji = false;
            CanUpload = true;
        }

        public string? UrlPathSegment { get; } = "createBotCrontabRule";
        public IScreen HostScreen { get; }
        public ObservableCollection<Response> Responses { get; set; }
        public ObservableCollection<Response> Emojis { get; set; }
        [Reactive] public string Crontab { get; set; }
        [Reactive] public ResponseMode ResponseMode { get; set; }
        [Reactive] public bool IsEmoji { get; set; }

        protected override CreateBotCrontabRuleDto CreateEntityInstance()
        {
            return new (IsEmoji, Crontab, Responses.Select(x => x.Value), Emojis.Select(x => x.Value), ResponseMode);
        }
    }
}