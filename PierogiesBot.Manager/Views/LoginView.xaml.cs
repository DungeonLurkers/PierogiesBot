using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using PierogiesBot.Manager.Models.Messages;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;

namespace PierogiesBot.Manager.Views
{
    public partial class LoginView
    {
        public LoginView(LoginViewModel viewModel, IMessageBus messageBus)
        {
            InitializeComponent();

            ViewModel = viewModel;

            this.WhenActivated(disposable =>
            {
                CancelButton
                    .Events().Click
                    .Do(_ => messageBus.SendMessage(new CloseApplication()))
                    .Subscribe()
                    .DisposeWith(disposable);

                SignInButton
                    .Events().Click
                    .Select(_ => (UserNameBox.Text, PasswordBox.SecurePassword))
                    .InvokeCommand(viewModel.SignInCommand)
                    .DisposeWith(disposable);

                SignAsSavedButton
                    .Events().Click
                    .Select(_ => Unit.Default)
                    .InvokeCommand(viewModel.SignInFromSettingsCommand)
                    .DisposeWith(disposable);
            });
        }
    }
}