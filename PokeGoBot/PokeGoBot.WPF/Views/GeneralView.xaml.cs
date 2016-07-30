using System.Windows.Controls;
using System.Windows.Input;
using PokeGoBot.WPF.Viewmodels;

namespace PokeGoBot.WPF.Views
{
    /// <summary>
    /// Interaction logic for GeneralView.xaml
    /// </summary>
    public partial class GeneralView : UserControl
    {
        public GeneralView()
        {
            InitializeComponent();
        }

        private void TxtPassword_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = (GeneralViewModel) DataContext;
            viewModel.Password = txtPassword.Password;
        }
    }
}
