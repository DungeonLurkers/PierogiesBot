using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Commons.Enums;
using PierogiesBot.Manager.Models;
using PierogiesBot.Manager.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PierogiesBot.Manager.ViewModels.CreateRules
{
    public class CreateReactRuleViewModel : CreateRuleViewModelBase<CreateBotReactRuleDto>, IRoutableViewModel
    {
        public CreateReactRuleViewModel(IScreen hostScreen, IPierogiesBotService botService,
            ILogger<CreateReactRuleViewModel> logger) : base(botService, logger)
        {
            HostScreen = hostScreen;

            ResponseMode = ResponseMode.First;
            StringComparison = StringComparison.InvariantCultureIgnoreCase;
            Reactions = new ObservableCollection<Response>();
            TriggerText = "";
            IsTriggerTextRegex = false;
            ShouldTriggerOnContains = false;
        }

        [Reactive] public ResponseMode ResponseMode { get; set; }
        [Reactive] public StringComparison StringComparison { get; set; }
        public ObservableCollection<Response> Reactions { get; set; }
        [Reactive] public string TriggerText { get; set; }
        [Reactive] public bool IsTriggerTextRegex { get; set; }
        [Reactive] public bool ShouldTriggerOnContains { get; set; }


        public string? UrlPathSegment { get; } = "createResponseRule";
        public IScreen HostScreen { get; }
        [Reactive] public bool CanUpload { get; set; }

        protected override CreateBotReactRuleDto CreateEntityInstance()
        {
            return new(Reactions.Select(x => x.Value), TriggerText, StringComparison, IsTriggerTextRegex,
                ShouldTriggerOnContains, ResponseMode);
        }
    }
}