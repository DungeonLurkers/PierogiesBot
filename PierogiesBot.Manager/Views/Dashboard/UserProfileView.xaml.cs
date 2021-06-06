using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;

namespace PierogiesBot.Manager.Views.Dashboard
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

        public UserProfileView() : this(App.Container.GetRequiredService<UserProfileViewModel>())
        {
        }
    }
}