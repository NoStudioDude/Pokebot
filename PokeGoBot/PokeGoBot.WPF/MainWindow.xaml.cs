using System;
using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls;

namespace PokeGoBot.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Title = $"Poke Go Bot v{Configuration.Version()}";
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
