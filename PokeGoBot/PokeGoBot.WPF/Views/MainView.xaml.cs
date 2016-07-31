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

        private void LvLogs_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            lvLogs.ScrollIntoView(lvLogs.Items.Count - 1);
        }
    }
}
