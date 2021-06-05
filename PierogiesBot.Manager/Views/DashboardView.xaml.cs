using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using PierogiesBot.Manager.Models.Messages;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;

namespace PierogiesBot.Manager.Views
{
    public partial class DashboardView
    {
        public DashboardView(DashboardViewModel viewModel, Lazy<UserProfileView> userProfileView, Lazy<ResponseRulesView> responseRulesView, IMessageBus messageBus)
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
                    
                   AddResponseRuleButton.Events().Click
                       .Select(_ => Unit.Default)
                       .InvokeCommand(ViewModel.GoToCreateResponseRuleCommand)
                       .DisposeWith(disposable);
            });
        }
    }
}