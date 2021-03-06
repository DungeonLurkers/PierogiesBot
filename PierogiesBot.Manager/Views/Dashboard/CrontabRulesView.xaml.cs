﻿using System.Reactive.Disposables;
using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;

namespace PierogiesBot.Manager.Views.Dashboard
{
    public partial class CrontabRulesView
    {
        public CrontabRulesView(CrontabRulesViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.CrontabRules, x => x.RulesDataGrid.ItemsSource)
                    .DisposeWith(disposable);
            });
        }

        public CrontabRulesView() : this(App.Container.GetRequiredService<CrontabRulesViewModel>())
        {
        }
    }
}