using System.Windows;
using System.Windows.Controls;

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

        private void DgLogs_OnAddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            DgLogs.ScrollIntoView(DgLogs.Items.Count - 1);
        }
    }
}