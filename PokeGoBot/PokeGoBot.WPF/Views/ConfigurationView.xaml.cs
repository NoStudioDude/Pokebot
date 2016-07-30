using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PokeGoBot.WPF.Viewmodels;

namespace PokeGoBot.WPF.Views
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }

        private void TxtPassword_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = (ConfigurationViewModel)DataContext;
            viewModel.Password = txtPassword.Password;
        }

        private void ConfigurationView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (ConfigurationViewModel)DataContext;
            txtPassword.Password = viewModel.Password;
        }
    }
}
