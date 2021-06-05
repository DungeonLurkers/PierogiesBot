using System.Windows.Controls;
using PierogiesBot.Manager.ViewModels;

namespace PierogiesBot.Manager.Views
{
    public partial class CreateResponseRuleView
    {
        public CreateResponseRuleView(CreateResponseRuleViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }
    }
}