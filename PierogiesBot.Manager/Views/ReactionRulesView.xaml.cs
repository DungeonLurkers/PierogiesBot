﻿using System.Reactive.Disposables;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;

namespace PierogiesBot.Manager.Views
{
    public partial class ReactionRulesView
    {
        public ReactionRulesView(ReactionRulesViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.ReactionRules, x => x.RulesDataGrid.ItemsSource)
                    .DisposeWith(disposable);
            });
        }

        public ReactionRulesView() : this(App.Container.GetRequiredService<ReactionRulesViewModel>())
        {
            
        }
    }
}