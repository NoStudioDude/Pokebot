using System;
using System.ComponentModel;
using MahApps.Metro.Controls;
using PokeGoBot.Core;

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
