using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using PierogiesBot.Manager.Models.Messages;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;

namespace PierogiesBot.Manager.Views.Dashboard
{
    public partial class DashboardView
    {
        public DashboardView(DashboardViewModel viewModel, Lazy<UserProfileView> userProfileView,
            Lazy<ResponseRulesView> responseRulesView, IMessageBus messageBus)
        {
            InitializeComponent();

            ViewModel = viewModel;

            this.WhenActivated(disposable =>
            {
                ProfileTab.Content = userProfileView.Value;
                ResponseRulesTab.Content = responseRulesView.Value;

                RefreshDataButton
                    .Events().Click
                    .Do(_ => messageBus.SendMessage(new RefreshData()))
                    .Subscribe()
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel, vm => vm.GoToCreateResponseRuleCommand, v => v.AddResponseRuleButton)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, vm => vm.GoToCreateReactionRuleCommand, v => v.AddReactionRuleButton)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, vm => vm.GoToCreateCrontabRuleCommand, v => v.AddCrontabRuleButton)
                    .DisposeWith(disposable);
            });
        }
    }
}