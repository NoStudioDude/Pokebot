using System.Windows;
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

        private void LvLogs_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            lvLogs.ScrollIntoView(lvLogs.Items.Count -1);
        }
    }
}
