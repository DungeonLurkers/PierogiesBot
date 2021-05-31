using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using PierogiesBot.Manager.Models.Messages;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;

namespace PierogiesBot.Manager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IMessageBus _messageBus;

        public MainWindow(MainWindowViewModel viewModel, IMessageBus messageBus)
        {
            _messageBus = messageBus;
            InitializeComponent();
            
            ViewModel = viewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router)
                    .DisposeWith(disposable);

                _messageBus.Listen<CloseApplication>()
                    .Do(_ => Close())
                    .Subscribe()
                    .DisposeWith(disposable);

                ViewModel.CheckIsAuthenticated.Execute().Subscribe().DisposeWith(disposable);
            });
        }
    }
}