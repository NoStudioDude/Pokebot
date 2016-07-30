using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PokeGoBot.WPF.Viewmodels;

namespace PokeGoBot.WPF.Views
{
    /// <summary>
    ///     Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        private MainViewModel _mainViewModel;

        public MainView()
        {
            InitializeComponent();
        }

        private void TxtPTCPassword_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            //_mainViewModel = (MainViewModel) DataContext;
            //_mainViewModel.PtcPassword = txtPTCPassword.Password;
        }

        private void TxtGooglePassword_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            //_mainViewModel = (MainViewModel) DataContext;
            //_mainViewModel.GooglePassword = txtGooglePassword.Password;
        }

        private void MainView_OnLoaded(object sender, RoutedEventArgs e)
        {
            //this.dudLatitude.Value = 40.417426;
            //this.dudLongitude.Value = -3.683230;
        }
    }
}
