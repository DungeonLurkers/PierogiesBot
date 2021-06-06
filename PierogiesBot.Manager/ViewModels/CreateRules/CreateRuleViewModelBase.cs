using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos;
using PierogiesBot.Manager.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PierogiesBot.Manager.ViewModels.CreateRules
{
    public abstract class CreateRuleViewModelBase<T> : ReactiveObject where T : ICreateRuleDto
    {
        private readonly IPierogiesBotService _botService;
        private readonly ILogger<CreateRuleViewModelBase<T>> _logger;

        public CreateRuleViewModelBase(IPierogiesBotService botService, ILogger<CreateRuleViewModelBase<T>> logger)
        {
            _botService = botService;
            _logger = logger;
            UploadNewRule = ReactiveCommand.CreateFromTask(Execute);


            UploadNewRule.IsExecuting.Select(b => !b).ToProperty(this, vm => vm.CanUpload);
            CanUpload = true;
        }

        public ReactiveCommand<Unit, Unit> UploadNewRule { get; }
        [Reactive] public bool CanUpload { get; set; }

        protected abstract T CreateEntityInstance();

        private async Task Execute()
        {
            _logger.LogInformation("Uploading new rule...");
            await _botService.UploadRule(CreateEntityInstance());
            _logger.LogInformation("New rule uploaded");
        }
    }
}