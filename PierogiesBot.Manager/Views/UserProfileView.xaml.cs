using System;
using System.Reactive.Disposables;
using System.Windows.Controls;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;

namespace PierogiesBot.Manager.Views
{
    public partial class UserProfileView
    {
        public UserProfileView(UserProfileViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, vm => vm.UserName, v => v.UserNameLabel.Content)
                    .DisposeWith(disposable);
                
                ViewModel.LoadCurrentUserData.Execute().Subscribe().DisposeWith(disposable);
            });
        }
    }
}