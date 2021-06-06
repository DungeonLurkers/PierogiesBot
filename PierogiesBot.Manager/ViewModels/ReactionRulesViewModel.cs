using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DynamicData.Binding;
using PierogiesBot.Manager.Models;
using PierogiesBot.Manager.Models.Messages;
using PierogiesBot.Manager.Services;
using ReactiveUI;

namespace PierogiesBot.Manager.ViewModels
{
    public class ReactionRulesViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IPierogiesBotService _botService;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;

        public ReactionRulesViewModel(IScreen hostScreen, IPierogiesBotService botService, IMessageBus messageBus,
            IMapper mapper)
        {
            _botService = botService;
            _messageBus = messageBus;
            _mapper = mapper;
            HostScreen = hostScreen;

            LoadRules = ReactiveCommand.CreateFromTask(Execute);
            ReactionRules = new ObservableCollectionExtended<ReactionRuleModel>();

            _messageBus.ListenIncludeLatest<RefreshData>()
                .Where(x => x is not null)
                .Select(x => Unit.Default)
                .InvokeCommand(LoadRules);
        }

        public ObservableCollectionExtended<ReactionRuleModel> ReactionRules { get; set; }

        public ReactiveCommand<Unit, Unit> LoadRules { get; }

        public string? UrlPathSegment => "reactionRules";
        public IScreen HostScreen { get; }

        private async Task Execute()
        {
            var rules = await _botService.GetBotReactRules();
            if (rules is not null)
            {
                ReactionRules.Clear();
                var rulesMapped = rules.Select(x => _mapper.Map<ReactionRuleModel>(x));
                ReactionRules.AddRange(rulesMapped);
            }
        }
    }
}