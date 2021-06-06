using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Commons.Enums;
using PierogiesBot.Manager.Models;
using PierogiesBot.Manager.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PierogiesBot.Manager.ViewModels.CreateRules
{
    public class CreateResponseRuleViewModel : CreateRuleViewModelBase<CreateBotResponseRuleDto>, IRoutableViewModel
    {
        private readonly IPierogiesBotService _botService;
        private readonly ILogger<CreateResponseRuleViewModel> _logger;
        private readonly INavigationService _navigationService;

        public CreateResponseRuleViewModel(IScreen hostScreen, IPierogiesBotService botService,
            ILogger<CreateResponseRuleViewModel> logger, INavigationService navigationService) : base(botService,
            logger)
        {
            _botService = botService;
            _logger = logger;
            _navigationService = navigationService;
            HostScreen = hostScreen;

            ResponseMode = ResponseMode.First;
            Responses = new ObservableCollection<Response>();
            TriggerText = "";
            IsTriggerTextRegex = false;
            ShouldTriggerOnContains = false;
        }

        [Reactive] public ResponseMode ResponseMode { get; set; }
        [Reactive] public StringComparison StringComparison { get; set; }
        public ObservableCollection<Response> Responses { get; set; }
        [Reactive] public string TriggerText { get; set; }
        [Reactive] public bool IsTriggerTextRegex { get; set; }
        [Reactive] public bool ShouldTriggerOnContains { get; set; }


        public string? UrlPathSegment { get; } = "createResponseRule";
        public IScreen HostScreen { get; }
        [Reactive] public bool CanUpload { get; set; }

        protected override CreateBotResponseRuleDto CreateEntityInstance()
        {
            return new(ResponseMode, Responses.Select(x => x.Value), TriggerText, StringComparison,
                IsTriggerTextRegex, ShouldTriggerOnContains);
        }
    }
}