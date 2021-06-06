using System.Reactive.Disposables;
using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;

namespace PierogiesBot.Manager.Views.Dashboard
{
    public partial class ResponseRulesView
    {
        public ResponseRulesView(ResponseRulesViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.ResponseRules, x => x.RulesDataGrid.ItemsSource)
                    .DisposeWith(disposable);
            });
        }

        public ResponseRulesView() : this(App.Container.GetRequiredService<ResponseRulesViewModel>())
        {
        }
    }
}