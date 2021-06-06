using System;
using System.Reactive.Disposables;
using PierogiesBot.Commons.Enums;
using PierogiesBot.Manager.ViewModels.CreateRules;
using ReactiveUI;

namespace PierogiesBot.Manager.Views.CreateRules
{
    public partial class CreateResponseRuleView
    {
        public CreateResponseRuleView(CreateResponseRuleViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;

            this.WhenActivated(WhenActivated);
        }

        private void WhenActivated(CompositeDisposable disposable)
        {
            ResponseModeComboBox.ItemsSource = Enum.GetValues<ResponseMode>();
            ResponseModeComboBox.SelectedItem = ResponseMode.First;

            StringComparisonComboBox.ItemsSource = Enum.GetValues<StringComparison>();
            StringComparisonComboBox.SelectedItem = StringComparison.InvariantCultureIgnoreCase;

            ResponsesListView.ItemsSource = ViewModel.Responses;

            this.Bind(ViewModel, vm => vm.TriggerText, v => v.TriggerTextBox.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.ResponseMode,
                    v => (ResponseMode) v.ResponseModeComboBox.SelectedItem)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.IsTriggerTextRegex, v => v.IsRegexCheckBox.IsChecked)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.ShouldTriggerOnContains, v => v.TriggerOnContainsCheckbox.IsChecked)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.StringComparison, v => v.StringComparisonComboBox.SelectedItem)
                .DisposeWith(disposable);

            this.BindCommand(ViewModel, vm => vm.UploadNewRule, v => v.UploadRuleButton)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.CanUpload, v => v.UploadRuleButton.IsEnabled);
        }
    }
}